import { computed, inject, Injectable, signal } from '@angular/core';
import { ClientHttpService } from './client-http.service';
import { ClientQueryParameters } from '../models/client-query-parameters';
import { ClientSseService } from './client-sse.service';
import { ClientResponse } from '../models/client-response';

@Injectable({
  providedIn: 'root',
})
export class ClientStore {
  private readonly clientHttpService = inject(ClientHttpService);
  private readonly clientSseService = inject(ClientSseService);

  queryParameters = signal<ClientQueryParameters | undefined>(undefined);
  selectedLogin = signal<string>('');

  created = signal<ClientResponse | null>(null);
  updated = signal<ClientResponse | null>(null);
  deleted = signal<ClientResponse | null>(null);

  private pagedListResource = this.clientHttpService.getPagedListResource(this.queryParameters);

  private selectedResource = this.clientHttpService.getByLogin(this.selectedLogin);

  selected = this.selectedResource.value;
  pagedListIsLoading = this.pagedListResource.isLoading;
  selectedIsLoading = this.selectedResource.isLoading;
  pagedListError = this.pagedListResource.error;
  selectedError = this.selectedResource.error;
  pagedList = computed(() => this.pagedListResource.value() ?? []);
  pagingData = computed(() => JSON.parse(this.pagedListResource.headers()?.get('x-pagination') || '{}'));

  constructor() {
    this.clientSseService.connect({
      onCreated: (client) => this.created.set(client),
      onUpdated: (client) => this.updated.set(client),
      onDeleted: (client) => this.deleted.set(client),
    });
  }
}
