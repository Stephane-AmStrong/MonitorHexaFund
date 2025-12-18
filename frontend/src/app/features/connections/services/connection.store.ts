import { computed, inject, Injectable, signal } from '@angular/core';
import { ConnectionService } from './connection.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { ConnectionQueryParameters } from '../models/connection-query-parameters';

@Injectable({
  providedIn: 'root',
})
export class ConnectionStore {
  private connectionService = inject(ConnectionService);

  queryParameters = signal<ConnectionQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedId = signal<string | undefined>(undefined);

  private pagedListResource = rxResource({
    params: () => (this.queryParameters()),
    stream: ({params}) => this.connectionService.getPagedListByQueryAsync(params),
    defaultValue: [],
  })

  private selectedResource = rxResource({
    params: () => (this.selectedId()),
    stream: ({params}) => this.connectionService.getById(params),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(() => this.pagedListResource.isLoading() || this.selectedResource.isLoading());
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());
}
