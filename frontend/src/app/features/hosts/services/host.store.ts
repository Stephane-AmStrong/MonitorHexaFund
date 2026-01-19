import { computed, inject, Injectable, signal } from '@angular/core';
import { HostHttpService } from './host-http.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { HostQueryParameters } from '../models/host-query-parameters';
import { HostSseService } from './host-sse.service';
import { HostDetailedResponse } from '../models/host-detailed-response';

@Injectable({
  providedIn: 'root',
})
export class HostStore {
  private readonly hostHttpService = inject(HostHttpService);
  private readonly hostSseService = inject(HostSseService);

  queryParameters = signal<HostQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedName = signal<string | undefined>(undefined);

  created = signal<HostDetailedResponse | null>(null);
  updated = signal<HostDetailedResponse | null>(null);
  deleted = signal<HostDetailedResponse | null>(null);

  private pagedListResource = rxResource({
    params: () => this.queryParameters(),
    stream: ({ params }) => this.hostHttpService.getPagedListByQueryAsync(params),
    defaultValue: [],
  });

  private selectedResource = rxResource({
    params: () => this.selectedName(),
    stream: ({ params }) => this.hostHttpService.getByName(params),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(
    () => this.pagedListResource.isLoading() || this.selectedResource.isLoading()
  );
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());

  constructor() {
    this.hostSseService.connect({
      onCreated: (entity) => this.created.set(entity as HostDetailedResponse),
      onUpdated: (entity) => this.updated.set(entity as HostDetailedResponse),
      onDeleted: (entity) => this.deleted.set(entity as HostDetailedResponse),
    });
  }
}
