import { computed, inject, Injectable, signal } from '@angular/core';
import { ServerQueryParameters } from '../models/server-query-parameters';
import { ServerService } from './server.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { HostService } from '../../hosts/services/host.service';

@Injectable({
  providedIn: 'root',
})
export class ServerStore {
  private serverService = inject(ServerService);
  private hostService = inject(HostService);

  queryParameters = signal<ServerQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedId = signal<[hostName: string, serverName: string] | undefined>(undefined);

  private pagedListResource = rxResource({
    params: () => (this.queryParameters()),
    stream: ({params}) => this.serverService.getPagedListByQueryAsync(params),
    defaultValue: [],
  })

  private selectedResource = rxResource({
    params: () => (this.selectedId()),
    stream: ({params: [hostName, serverName]}) => this.hostService.getServerByHostAndApp(hostName, serverName),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(() => this.pagedListResource.isLoading() || this.selectedResource.isLoading());
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());
}
