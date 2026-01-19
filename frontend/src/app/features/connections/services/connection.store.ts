import { computed, inject, Injectable, signal } from '@angular/core';
import { ConnectionHttpService } from './connection-http.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { ConnectionQueryParameters } from '../models/connection-query-parameters';
import { ConnectionResponse } from '../models/connection-response';
import { ConnectionSseService } from './connection-sse.service';

@Injectable({
  providedIn: 'root',
})
export class ConnectionStore {
  private readonly connectionHttpService = inject(ConnectionHttpService);
  private readonly connectionSseService = inject(ConnectionSseService);

  queryParameters = signal<ConnectionQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedId = signal<string | undefined>(undefined);

  created = signal<ConnectionResponse | null>(null);
  updated = signal<ConnectionResponse | null>(null);
  deleted = signal<ConnectionResponse | null>(null);

  private pagedListResource = rxResource({
    params: () => this.queryParameters(),
    stream: ({ params }) => this.connectionHttpService.getPagedListByQueryAsync(params),
    defaultValue: [],
  });

  private selectedResource = rxResource({
    params: () => this.selectedId(),
    stream: ({ params }) => this.connectionHttpService.getById(params),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(
    () => this.pagedListResource.isLoading() || this.selectedResource.isLoading()
  );
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());

  constructor() {
    this.connectionSseService.connect({
      onCreated: (entity) => this.created.set(entity),
      onUpdated: (entity) => this.updated.set(entity),
      onDeleted: (entity) => this.deleted.set(entity),
    });
  }
}
