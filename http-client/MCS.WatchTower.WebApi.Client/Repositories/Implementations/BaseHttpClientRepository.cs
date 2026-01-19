#nullable enable
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MCS.WatchTower.WebApi.DataTransferObjects;
using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace MCS.WatchTower.WebApi.Client.Repositories.Implementations;

internal abstract class BaseHttpClientRepository(
    HttpClient httpClient,
    ILogger logger,
    string endpoint,
    string entityName)
{
    private readonly bool _isValid = ValidateParameters(httpClient, logger, endpoint, entityName);

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    private readonly string _endpoint = $"{httpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";

    private static bool ValidateParameters(HttpClient? httpClient, ILogger? logger, string endpoint, string entityName)
    {
        if (httpClient is null)
        {
            logger?.LogError("HttpClient cannot be null");
            return false;
        }

        if (httpClient.BaseAddress == null)
        {
            logger?.LogError("HttpClient BaseAddress is not configured");
            return false;
        }

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            logger?.LogError("Endpoint cannot be null or empty for {EntityName}", entityName);
            return false;
        }

        if (string.IsNullOrWhiteSpace(entityName))
        {
            logger?.LogError("EntityName cannot be null or empty");
            return false;
        }

        return true;
    }


    // READ (Get paged list)
    protected async Task<PagedList<TResponse>?> BaseGetPagedListAsync<TResponse>(QueryParameters query, CancellationToken cancellationToken) where TResponse : IBaseDto
    {
        if (!_isValid)
        {
            logger.LogError("Repository is not properly initialized. Cannot proceed with GET paged list for {EntityName}s", entityName ?? "unknown");
            return CreateEmptyPagedList<TResponse>();
        }

        logger.LogDebug("Starting GET paged list for {EntityName}s", entityName);

        try
        {
            string queryString = BuildQueryString(query);
            var endpoint = string.IsNullOrEmpty(queryString) ? _endpoint : $"{_endpoint}?{queryString}";

            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await LogHttpErrorAsync(response, $"retrieving {entityName}s", cancellationToken);
                return CreateEmptyPagedList<TResponse>();
            }

            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var dataArray = await JsonSerializer.DeserializeAsync<TResponse[]>(contentStream, _options, cancellationToken);

            var paginationMetadata = ExtractPaginationMetadata(response);

            var pagedList = new PagedList<TResponse>(dataArray?.ToList() ?? new List<TResponse>(), paginationMetadata);

            logger.LogDebug("Successfully retrieved {EntityCount} {EntityName}s (Page {CurrentPage}/{TotalPages})", paginationMetadata.TotalCount, entityName, paginationMetadata.CurrentPage, paginationMetadata.TotalPages);

            return pagedList;
        }
        catch (Exception ex)
        {
            return HandleException(ex, $"retrieving {entityName}s", cancellationToken, CreateEmptyPagedList<TResponse>());
        }
    }

    // READ (Get by ID)
    protected async Task<TResponse?> BaseGetByIdAsync<TResponse>(string entityId, CancellationToken cancellationToken) where TResponse : IBaseDto
    {
        if (!_isValid)
        {
            logger?.LogError("Repository is not properly initialized. Cannot proceed with GET {EntityName}", entityName ?? "unknown");
            return default(TResponse);
        }

        if (string.IsNullOrWhiteSpace(entityId)) throw new ArgumentException($"{entityName} ID cannot be null or empty", nameof(entityId));

        logger.LogDebug("Starting GET for {EntityName} {EntityId}", entityName, entityId);

        string getByIdUrl = $"{_endpoint}/{entityId}";

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, getByIdUrl);
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await LogHttpErrorAsync(response, $"retrieving {entityName} {entityId}", cancellationToken);
                return default(TResponse);
            }

            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var entity = await JsonSerializer.DeserializeAsync<TResponse>(contentStream, _options, cancellationToken);

            logger.LogDebug("Successfully retrieved {EntityName} with ID {EntityId}", entityName, entityId);
            return entity;
        }
        catch (Exception ex)
        {
            return HandleException<TResponse>(ex, $"retrieving {entityName} {entityId}", cancellationToken);
        }
    }

    // CREATE
    protected async Task<TResponse?> BaseCreateAsync<TRequest, TResponse>(TRequest createRequest, CancellationToken cancellationToken) where TRequest : class where TResponse : IBaseDto
    {
        if (!_isValid)
        {
            logger?.LogError("Repository is not properly initialized. Cannot proceed with CREATE {EntityName}", entityName ?? "unknown");
            return default(TResponse);
        }

        ArgumentNullException.ThrowIfNull(createRequest);

        logger.LogDebug("Starting CREATE for {EntityName}", entityName);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, _endpoint);
            request.Content = JsonContent.Create(createRequest, options: _options);

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await LogHttpErrorAsync(response, $"creating {entityName}", cancellationToken);
                return default(TResponse);
            }

            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var createdEntity = await JsonSerializer.DeserializeAsync<TResponse>(contentStream, _options, cancellationToken);

            logger.LogDebug("Successfully created {EntityName} with ID {EntityId}", entityName, createdEntity?.Id);
            return createdEntity;
        }
        catch (Exception ex)
        {
            return HandleException<TResponse>(ex, $"creating {entityName}", cancellationToken);
        }
    }

    // UPDATE
    protected async Task<bool> BaseUpdateAsync<TRequest>(string entityId, TRequest updateRequest, CancellationToken cancellationToken) where TRequest : class
    {
        if (!_isValid)
        {
            logger?.LogError("Repository is not properly initialized. Cannot proceed with UPDATE {EntityName}", entityName ?? "unknown");
            return false;
        }

        if (string.IsNullOrWhiteSpace(entityId)) throw new ArgumentException($"{entityName} ID cannot be null or empty", nameof(entityId));

        ArgumentNullException.ThrowIfNull(updateRequest);

        logger.LogDebug("Starting UPDATE for {EntityName} {EntityId}", entityName, entityId);

        string updateUrl = $"{_endpoint}/{entityId}";

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, updateUrl);
            request.Content = JsonContent.Create(updateRequest, options: _options);

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await LogHttpErrorAsync(response, $"updating {entityName} {entityId}", cancellationToken);
                return false;
            }

            logger.LogDebug("Successfully updated {EntityName} with ID {EntityId}", entityName, entityId);
            return true;
        }
        catch (Exception ex)
        {
            return HandleException(ex, $"updating {entityName} {entityId}", cancellationToken, false);
        }
    }

    // DELETE
    protected async Task<bool> BaseDeleteAsync(string entityId, CancellationToken cancellationToken)
    {
        if (!_isValid)
        {
            logger?.LogError("Repository is not properly initialized. Cannot proceed with DELETE {EntityName}", entityName ?? "unknown");
            return false;
        }

        if (string.IsNullOrWhiteSpace(entityId)) throw new ArgumentException($"{entityName} ID cannot be null or empty", nameof(entityId));

        logger.LogDebug("Starting DELETE for {EntityName} {EntityId}", entityName, entityId);

        string deleteUrl = $"{_endpoint}/{entityId}";

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, deleteUrl);
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await LogHttpErrorAsync(response, $"deleting {entityName} {entityId}", cancellationToken);
                return false;
            }

            logger.LogDebug("Successfully deleted {EntityName} with ID {EntityId}", entityName, entityId);
            return true;
        }
        catch (Exception ex)
        {
            return HandleException(ex, $"deleting {entityName} {entityId}", cancellationToken, false);
        }
    }

    private async Task LogHttpErrorAsync(HttpResponseMessage response, string operation, CancellationToken cancellationToken)
    {
        try
        {
            await using var errorStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(errorStream);
            var errorContent = await reader.ReadToEndAsync();

            logger.LogError("HTTP {StatusCode} error while {Operation}. Content: {ErrorContent}", (int)response.StatusCode, operation, errorContent);
        }
        catch
        {
            logger.LogError("HTTP {StatusCode} error while {Operation}. (Could not read error content)", (int)response.StatusCode, operation);
        }
    }

    private T? HandleException<T>(Exception ex, string operation, CancellationToken cancellationToken, T? defaultValue = default)
    {
        switch (ex)
        {
            case OperationCanceledException when cancellationToken.IsCancellationRequested:
                logger.LogWarning("{Operation} operation was cancelled", operation);
                break;

            case TaskCanceledException { InnerException: TimeoutException } taskCanceled:
                logger.LogError(taskCanceled, "Timeout while {Operation}", operation);
                break;

            case HttpRequestException httpEx:
                logger.LogError(httpEx, "HTTP error while {Operation}", operation);
                break;

            case JsonException jsonEx:
                logger.LogError(jsonEx, "JSON error while {Operation}", operation);
                break;

            default:
                logger.LogError(ex, "Unexpected error while {Operation}", operation);
                break;
        }

        return defaultValue;
    }

    private PagingMetadata ExtractPaginationMetadata(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("x-pagination", out var paginationHeaderValues))
        {
            logger.LogWarning("Missing x-pagination header, using default pagination metadata");
            return new PagingMetadata(1, 10, 0);
        }

        var paginationHeader = paginationHeaderValues.FirstOrDefault();
        if (string.IsNullOrEmpty(paginationHeader))
        {
            logger.LogWarning("Empty x-pagination header, using default pagination metadata");
            return new PagingMetadata(1, 10, 0);
        }

        try
        {
            var paginationData = JsonSerializer.Deserialize<PagingMetadata>(paginationHeader, _options);
            return paginationData ?? new PagingMetadata(1, 10, 0);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize pagination metadata from header: {Header}", paginationHeader);
            return new PagingMetadata(1, 10, 0);
        }
    }

    private string BuildQueryString(QueryParameters query)
    {
        if (query is null) return string.Empty;

        var queryParams = new List<KeyValuePair<string, StringValues>>();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            queryParams.Add(KeyValuePair.Create(nameof(query.SearchTerm), new StringValues(query.SearchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(query.OrderBy))
        {
            queryParams.Add(KeyValuePair.Create(nameof(query.OrderBy), new StringValues(query.OrderBy)));
        }

        if (query.Page is not null && query.Page > 1)
        {
            queryParams.Add(KeyValuePair.Create(nameof(query.Page), new StringValues(query.Page.ToString())));
        }

        if (query.PageSize is not null && query.PageSize != 10)
        {
            queryParams.Add(KeyValuePair.Create(nameof(query.PageSize), new StringValues(query.PageSize.ToString())));
        }

        var specificParams = AddSpecificQueryParameters(query);

        if (specificParams?.Count > 0)
        {
            queryParams.AddRange(specificParams);
        }

        return queryParams.Count == 0 ? string.Empty :
               QueryString.Create(queryParams).ToString().TrimStart('?');
    }

    protected virtual List<KeyValuePair<string, StringValues>> AddSpecificQueryParameters(QueryParameters query)
    {
        return new List<KeyValuePair<string, StringValues>>();
    }

    private static PagedList<T> CreateEmptyPagedList<T>() where T : IBaseDto => new(new List<T>(), new(1, 10, 0));
}
