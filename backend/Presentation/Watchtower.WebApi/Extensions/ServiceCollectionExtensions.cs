using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Application.Models;
using Application.UseCases.Alerts.CreateOrIncrement;
using Application.UseCases.Alerts.Delete;
using Application.UseCases.Alerts.GetById;
using Application.UseCases.Alerts.GetByQuery;
using Application.UseCases.Alerts.Update;
using Application.UseCases.Clients.Create;
using Application.UseCases.Clients.Delete;
using Application.UseCases.Clients.GetByGaia;
using Application.UseCases.Clients.GetByLogin;
using Application.UseCases.Clients.GetByQuery;
using Application.UseCases.Clients.Update;
using Application.UseCases.Connections.Establish;
using Application.UseCases.Connections.GetById;
using Application.UseCases.Connections.GetByQuery;
using Application.UseCases.Connections.Terminate;
using Application.UseCases.Hosts.Create;
using Application.UseCases.Hosts.Delete;
using Application.UseCases.Hosts.GetById;
using Application.UseCases.Hosts.GetByName;
using Application.UseCases.Hosts.GetByQuery;
using Application.UseCases.Hosts.GetAppOfHost;
using Application.UseCases.Hosts.GetWithAppsByQuery;
using Application.UseCases.Hosts.Update;
using Application.UseCases.Apps.Create;
using Application.UseCases.Apps.Delete;
using Application.UseCases.Apps.GetById;
using Application.UseCases.Apps.GetByQuery;
using Application.UseCases.Apps.Update;
using Application.UseCases.AppStatuses.Create;
using Application.UseCases.AppStatuses.Delete;
using Application.UseCases.AppStatuses.GetById;
using Application.UseCases.AppStatuses.GetByQuery;
using Domain.Abstractions.Events;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared.Common;
using FluentValidation;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MongoDB.Driver;
using Persistence.Events;
using Persistence.Repository;
using Services;
using Watchtower.WebApi.Middleware;
using Watchtower.WebApi.Utilities;
using Host = Domain.Entities.Host;

namespace Watchtower.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        string[] allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                //builder.WithOrigins(allowedOrigins)
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });

        });
    }

    public static void ConfigureMongoDB(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB");
        var databaseName = configuration.GetConnectionString("DatabaseName");

        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            return new MongoClient(connectionString);
        });

        services.AddScoped(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });
    }

    public static void ConfigureFlatConfigurationSync(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFlatConfigurationService>(serviceProvider =>
        {
            string mcsConfigFlatPath = configuration["SharedFilePaths:McsConfigFlat"]!;
            var logger = serviceProvider.GetRequiredService<ILogger<FlatConfigurationService>>();
            var jsonReader = serviceProvider.GetRequiredService<IJsonFileReader>();

            return new FlatConfigurationService(logger, jsonReader, mcsConfigFlatPath);
        });
    }

    public static void ConfigureValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateOrIncrementAlertCommand>, CreateOrIncrementAlertValidator>();
        services.AddScoped<IValidator<UpdateAlertCommand>, UpdateAlertValidator>();
        services.AddScoped<IValidator<DeleteAlertCommand>, DeleteAlertValidator>();

        services.AddScoped<IValidator<CreateClientCommand>, CreateClientValidator>();
        services.AddScoped<IValidator<UpdateClientCommand>, UpdateClientValidator>();
        services.AddScoped<IValidator<DeleteClientCommand>, DeleteClientValidator>();

        services.AddScoped<IValidator<CreateHostCommand>, CreateHostValidator>();
        services.AddScoped<IValidator<UpdateHostCommand>, UpdateHostValidator>();
        services.AddScoped<IValidator<DeleteHostCommand>, DeleteHostValidator>();

        services.AddScoped<IValidator<EstablishConnectionCommand>, EstablishConnectionValidator>();
        services.AddScoped<IValidator<TerminateConnectionCommand>, TerminateConnectionValidator>();

        services.AddScoped<IValidator<CreateAppCommand>, CreateAppValidator>();
        services.AddScoped<IValidator<UpdateAppCommand>, UpdateAppValidator>();
        services.AddScoped<IValidator<DeleteAppCommand>, DeleteAppValidator>();

        services.AddScoped<IValidator<CreateAppStatusCommand>, CreateAppStatusValidator>();
        services.AddScoped<IValidator<DeleteAppStatusCommand>, DeleteAppStatusValidator>();
    }

    public static void ConfigureHandlers(this IServiceCollection services)
    {
        //Alerts
        services.AddScoped<IQueryHandler<GetAlertByIdQuery, AlertDetailedResponse>, GetAlertByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAlertQuery, PagedList<AlertResponse>>, GetAlertQueryHandler>();

        services.AddCommandWithValidation<CreateOrIncrementAlertCommand, CreateOrIncrementAlertCommandHandler, IValidator<CreateOrIncrementAlertCommand>, AlertResponse>();
        services.AddCommandWithValidation<UpdateAlertCommand, UpdateAlertCommandHandler, IValidator<UpdateAlertCommand>>();
        services.AddCommandWithValidation<DeleteAlertCommand, DeleteAlertCommandHandler, IValidator<DeleteAlertCommand>>();

        //Clients
        services.AddScoped<IQueryHandler<GetClientByGaiaQuery, ClientDetailedResponse>, GetClientByGaiaQueryHandler>();
        services.AddScoped<IQueryHandler<GetClientByLoginQuery, ClientDetailedResponse>, GetClientByLoginQueryHandler>();
        services.AddScoped<IQueryHandler<GetClientQuery, PagedList<ClientResponse>>, GetClientQueryHandler>();

        services.AddCommandWithValidation<CreateClientCommand, CreateClientCommandHandler, IValidator<CreateClientCommand>, ClientResponse>();
        services.AddCommandWithValidation<UpdateClientCommand, UpdateClientCommandHandler, IValidator<UpdateClientCommand>>();
        services.AddCommandWithValidation<DeleteClientCommand, DeleteClientCommandHandler, IValidator<DeleteClientCommand>>();

        //Connections
        services.AddScoped<IQueryHandler<GetConnectionByIdQuery, ConnectionDetailedResponse>, GetConnectionByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetConnectionQuery, PagedList<ConnectionResponse>>, GetConnectionQueryHandler>();

        services.AddCommandWithValidation<EstablishConnectionCommand, EstablishConnectionCommandHandler, IValidator<EstablishConnectionCommand>, ConnectionResponse>();
        services.AddCommandWithValidation<TerminateConnectionCommand, TerminateConnectionCommandHandler, IValidator<TerminateConnectionCommand>>();

        //Hosts
        services.AddScoped<IQueryHandler<GetHostByIdQuery, HostDetailedResponse>, GetHostByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetHostByNameQuery, HostDetailedResponse>, GetHostByNameQueryHandler>();
        services.AddScoped<IQueryHandler<GetAppOfHostQuery, AppDetailedResponse>, GetAppOfHostQueryHandler>();
        services.AddScoped<IQueryHandler<GetHostQuery, PagedList<HostResponse>>, GetHostQueryHandler>();
        services.AddScoped<IQueryHandler<GetHostWithAppsQuery, PagedList<HostDetailedResponse>>, GetHostWithAppsQueryHandler>();

        services.AddCommandWithValidation<CreateHostCommand, CreateHostCommandHandler, IValidator<CreateHostCommand>, HostResponse>();
        services.AddCommandWithValidation<UpdateHostCommand, UpdateHostCommandHandler, IValidator<UpdateHostCommand>>();
        services.AddCommandWithValidation<DeleteHostCommand, DeleteHostCommandHandler, IValidator<DeleteHostCommand>>();

        //AppStatuses
        services.AddScoped<IQueryHandler<GetAppStatusByIdQuery, AppStatusDetailedResponse>, GetAppStatusByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAppStatusQuery, PagedList<AppStatusResponse>>, GetAppStatusQueryHandler>();

        services.AddCommandWithValidation<CreateAppStatusCommand, CreateAppStatusCommandHandler, IValidator<CreateAppStatusCommand>, AppStatusResponse>();
        services.AddCommandWithValidation<DeleteAppStatusCommand, DeleteAppStatusCommandHandler, IValidator<DeleteAppStatusCommand>>();

        //Apps
        services.AddScoped<IQueryHandler<GetAppByIdQuery, AppDetailedResponse>, GetAppByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAppQuery, PagedList<AppResponse>>, GetAppQueryHandler>();

        services.AddCommandWithValidation<CreateAppCommand, CreateAppCommandHandler, IValidator<CreateAppCommand>, AppResponse>();
        services.AddCommandWithValidation<UpdateAppCommand, UpdateAppCommandHandler, IValidator<UpdateAppCommand>>();
        services.AddCommandWithValidation<DeleteAppCommand, DeleteAppCommandHandler, IValidator<DeleteAppCommand>>();
    }

    public static void ConfigureDomainEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IEventHandler<CreatedEvent<Alert>>, AlertCreatedEventHandler>();
        services.AddScoped<IEventHandler<UpdatedEvent<Alert>>, AlertUpdatedEventHandler>();
        services.AddScoped<IEventHandler<DeletedEvent<Alert>>, AlertDeletedEventHandler>();

        services.AddScoped<IEventHandler<CreatedEvent<Client>>, ClientCreatedEventHandler>();
        services.AddScoped<IEventHandler<UpdatedEvent<Client>>, ClientUpdatedEventHandler>();
        services.AddScoped<IEventHandler<DeletedEvent<Client>>, ClientDeletedEventHandler>();

        services.AddScoped<IEventHandler<CreatedEvent<Host>>, HostCreatedEventHandler>();
        services.AddScoped<IEventHandler<UpdatedEvent<Host>>, HostUpdatedEventHandler>();
        services.AddScoped<IEventHandler<DeletedEvent<Host>>, HostDeletedEventHandler>();

        services.AddScoped<IEventHandler<CreatedEvent<Connection>>, ConnectionEstablishedEventHandler>();
        services.AddScoped<IEventHandler<DeletedEvent<Connection>>, ConnectionTerminatedEventHandler>();

        services.AddScoped<IEventHandler<CreatedEvent<App>>, AppCreatedEventHandler>();
        services.AddScoped<IEventHandler<UpdatedEvent<App>>, AppUpdatedEventHandler>();
        services.AddScoped<IEventHandler<DeletedEvent<App>>, AppDeletedEventHandler>();

        services.AddScoped<IEventHandler<CreatedEvent<AppStatus>>, AppStatusCreatedEventHandler>();
        services.AddScoped<IEventHandler<DeletedEvent<AppStatus>>, AppStatusDeletedEventHandler>();
    }

    public static IServiceCollection AddEventStreaming(this IServiceCollection services)
    {
        services.AddSingleton<IEventStreamingService<AlertResponse>, EventStreamingService<AlertResponse>>();
        services.AddSingleton<IEventStreamingService<ClientResponse>, EventStreamingService<ClientResponse>>();
        services.AddSingleton<IEventStreamingService<ConnectionResponse>, EventStreamingService<ConnectionResponse>>();
        services.AddSingleton<IEventStreamingService<HostResponse>, EventStreamingService<HostResponse>>();
        services.AddSingleton<IEventStreamingService<AppStatusResponse>, EventStreamingService<AppStatusResponse>>();
        services.AddSingleton<IEventStreamingService<AppResponse>, EventStreamingService<AppResponse>>();

        SseStreamingOptions sseOptions = new(
            JsonOptions: new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            }
        );

        services.AddSingleton(provider => new SseStreamer<AlertResponse>(sseOptions, provider.GetRequiredService<ILogger<SseStreamer<AlertResponse>>>()));
        services.AddSingleton(provider => new SseStreamer<ClientResponse>(sseOptions, provider.GetRequiredService<ILogger<SseStreamer<ClientResponse>>>()));
        services.AddSingleton(provider => new SseStreamer<ConnectionResponse>(sseOptions, provider.GetRequiredService<ILogger<SseStreamer<ConnectionResponse>>>()));
        services.AddSingleton(provider => new SseStreamer<HostResponse>(sseOptions, provider.GetRequiredService<ILogger<SseStreamer<HostResponse>>>()));
        services.AddSingleton(provider => new SseStreamer<AppStatusResponse>(sseOptions, provider.GetRequiredService<ILogger<SseStreamer<AppStatusResponse>>>()));
        services.AddSingleton(provider => new SseStreamer<AppResponse>(sseOptions, provider.GetRequiredService<ILogger<SseStreamer<AppResponse>>>()));

        return services;
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAlertsRepository, AlertsRepository>();
        services.AddScoped<IClientsRepository, ClientsRepository>();
        services.AddScoped<IConnectionsRepository, ConnectionsRepository>();
        services.AddScoped<IHostsRepository, HostsRepository>();
        services.AddScoped<IAppStatusesRepository, AppStatusesRepository>();
        services.AddScoped<IAppsRepository, AppsRepository>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<HeartbeatListenerService>();
        services.AddSingleton<IHeartbeatListenerService>(provider => provider.GetRequiredService<HeartbeatListenerService>());
        services.AddHostedService<HeartbeatListenerService>(provider => provider.GetRequiredService<HeartbeatListenerService>());

        services.AddScoped<IAlertsService, AlertsService>();
        services.AddScoped<IClientsService, ClientsService>();
        services.AddScoped<IConnectionsService, ConnectionsService>();
        services.AddScoped<IEventsDispatcher, EventsDispatcher>();
        services.AddScoped<IHostsService, HostsService>();
        services.AddScoped<IJsonFileReader, JsonFileReader>();
        services.AddScoped<IHostConfigurationSyncService, HostConfigurationSyncService>();
        services.AddScoped<IAppConfigurationSyncService, AppConfigurationSyncService>();
        services.AddScoped<IAppsService, AppsService>();
        services.AddScoped<IAppStatusesService, AppStatusesService>();

        //HostedService
        services.AddHostedService<WatchtowerBootstrapService>();
    }

    public static void AddOpenApiServices(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
    }

    public static void UseOpenApiWithSwagger(this WebApplication app)
    {
        app.MapOpenApi("/WatchtowerWebApi/openapi/v1.json");

        // Configure OpenAPI mapping and Swagger UI
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("openapi/v1.json", "WatchTower Web API");
            options.RoutePrefix = "WatchtowerWebApi";
        });
    }

    public static void ConfigureJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    public static void ConfigureGlobalExceptionHandling(this IServiceCollection services)
    {
        services.AddScoped<EndpointLoggingMiddleware>();
        services.AddScoped<ExceptionHandlingMiddleware>();
    }

    public static IServiceCollection AddKestrelConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"));
        return services;
    }

    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<Filters.ValidationFilter<TRequest>>().ProducesValidationProblem();
    }
}
