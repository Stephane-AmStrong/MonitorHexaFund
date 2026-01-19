import { computed, inject, Injectable, signal } from '@angular/core';
import { ClientHttpService } from './client-http.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { ClientQueryParameters } from '../models/client-query-parameters';
import { ClientSseService } from './client-sse.service';
import { ClientResponse } from '../models/client-response';

@Injectable({
  providedIn: 'root',
})
export class ClientStore {
  private readonly clientHttpService = inject(ClientHttpService);
  private readonly clientSseService = inject(ClientSseService);

  queryParameters = signal<ClientQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedLogin = signal<string | undefined>(undefined);

  created = signal<ClientResponse | null>(null);
  updated = signal<ClientResponse | null>(null);
  deleted = signal<ClientResponse | null>(null);

  private pagedListResource = rxResource({
    params: () => this.queryParameters(),
    stream: ({ params }) => this.clientHttpService.getPagedListByQueryAsync(params),
    defaultValue: [],
  });

  private selectedResource = rxResource({
    params: () => this.selectedLogin(),
    stream: ({ params }) => this.clientHttpService.getByLogin(params),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(
    () => this.pagedListResource.isLoading() || this.selectedResource.isLoading()
  );
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());

  constructor() {
    this.clientSseService.connect({
      onCreated: (client) => this.created.set(client),
      onUpdated: (client) => this.updated.set(client),
      onDeleted: (client) => this.deleted.set(client),
    });
  }
}
