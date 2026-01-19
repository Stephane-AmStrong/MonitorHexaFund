using Application.Abstractions.Handlers;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Hosts.GetAppOfHost;

public record GetAppOfHostQuery(string HostName, string AppName) : IQuery<AppDetailedResponse?>;
