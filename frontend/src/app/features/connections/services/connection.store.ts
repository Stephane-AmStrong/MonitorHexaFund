import { computed, inject, Injectable, signal } from '@angular/core';
import { ConnectionHttpService } from './connection-http.service';
import { ConnectionQueryParameters } from '../models/connection-query-parameters';
import { ConnectionResponse } from '../models/connection-response';
import { ConnectionSseService } from './connection-sse.service';

@Injectable({
  providedIn: 'root',
})
export class ConnectionStore {
  private readonly connectionHttpService = inject(ConnectionHttpService);
  private readonly connectionSseService = inject(ConnectionSseService);

  queryParameters = signal<ConnectionQueryParameters | undefined>(undefined);
  selectedId = signal<string>('');

  created = signal<ConnectionResponse | null>(null);
  updated = signal<ConnectionResponse | null>(null);
  deleted = signal<ConnectionResponse | null>(null);

  private pagedListResource = this.connectionHttpService.getPagedListResource(this.queryParameters);

  private selectedResource = this.connectionHttpService.getById(this.selectedId);

  selected = this.selectedResource.value;
  pagedListIsLoading = this.pagedListResource.isLoading;
  selectedIsLoading = this.selectedResource.isLoading;
  pagedListError = this.pagedListResource.error;
  selectedError = this.selectedResource.error;
  pagedList = computed(() => this.pagedListResource.value() ?? []);
  pagingData = computed(() => JSON.parse(this.pagedListResource.headers()?.get('x-pagination') || '{}'));

  constructor() {
    this.connectionSseService.connect({
      onCreated: (entity) => this.created.set(entity),
      onUpdated: (entity) => this.updated.set(entity),
      onDeleted: (entity) => this.deleted.set(entity),
    });
  }
}
