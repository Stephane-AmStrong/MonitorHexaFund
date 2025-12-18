import { computed, inject, Injectable, signal } from '@angular/core';
import { ClientService } from './client.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { ClientQueryParameters } from '../models/client-query-parameters';

@Injectable({
  providedIn: 'root',
})
export class ClientStore {
  private clientService = inject(ClientService);

  queryParameters = signal<ClientQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedLogin = signal<string | undefined>(undefined);

  private pagedListResource = rxResource({
    params: () => (this.queryParameters()),
    stream: ({params}) => this.clientService.getPagedListByQueryAsync(params),
    defaultValue: [],
  })

  private selectedResource = rxResource({
  params: () => (this.selectedLogin()),
    stream: ({params}) => this.clientService.getByLogin(params),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(() => this.pagedListResource.isLoading() || this.selectedResource.isLoading());
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());
}
