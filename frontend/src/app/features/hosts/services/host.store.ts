import { computed, inject, Injectable, signal } from '@angular/core';
import { HostHttpService } from './host-http.service';
import { HostQueryParameters } from '../models/host-query-parameters';
import { HostSseService } from './host-sse.service';
import { HostDetailedResponse } from '../models/host-detailed-response';

@Injectable({
  providedIn: 'root',
})
export class HostStore {
  private readonly hostHttpService = inject(HostHttpService);
  private readonly hostSseService = inject(HostSseService);

  queryParameters = signal<HostQueryParameters | undefined>(undefined);
  selectedName = signal<string>('');

  created = signal<HostDetailedResponse | null>(null);
  updated = signal<HostDetailedResponse | null>(null);
  deleted = signal<HostDetailedResponse | null>(null);

  private pagedListResource = this.hostHttpService.getPagedListResource(this.queryParameters);

  private selectedResource = this.hostHttpService.getByName(this.selectedName);

  selected = this.selectedResource.value;
  pagedListIsLoading = this.pagedListResource.isLoading;
  selectedIsLoading = this.selectedResource.isLoading;
  pagedListError = this.pagedListResource.error;
  selectedError = this.selectedResource.error;
  pagedList = computed(() => this.pagedListResource.value() ?? []);
  pagingData = computed(() => JSON.parse(this.pagedListResource.headers()?.get('x-pagination') || '{}'));

  constructor() {
    this.hostSseService.connect({
      onCreated: (entity) => this.created.set(entity as HostDetailedResponse),
      onUpdated: (entity) => this.updated.set(entity as HostDetailedResponse),
      onDeleted: (entity) => this.deleted.set(entity as HostDetailedResponse),
    });
  }
}
