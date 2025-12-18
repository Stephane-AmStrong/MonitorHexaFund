import { computed, inject, Injectable, signal } from '@angular/core';
import { ServerStatusService } from './server-status.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { ServerStatusQueryParameters } from '../models/server-status-query-parameters';

@Injectable({
  providedIn: 'root',
})
export class ServerStatusStore {
  private serverStatusService = inject(ServerStatusService);

  queryParameters = signal<ServerStatusQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedId = signal<string | undefined>(undefined);

  private pagedListResource = rxResource({
    params: () => (this.queryParameters()),
    stream: ({params}) => this.serverStatusService.getPagedListByQueryAsync(params),
    defaultValue: [],
  })

  private selectedResource = rxResource({
    params: () => (this.selectedId()),
    stream: ({params}) => this.serverStatusService.getById(params),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(() => this.pagedListResource.isLoading() || this.selectedResource.isLoading());
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());
}
