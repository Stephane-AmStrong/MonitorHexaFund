import { computed, inject, Injectable, signal } from '@angular/core';
import { AppStatusHttpService } from './app-status-http.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { AppStatusQueryParameters } from '../models/app-status-query-parameters';
import { AppStatusSseService } from './app-status-sse.service';
import { AppStatusResponse } from '../models/app-status-response';

@Injectable({
  providedIn: 'root',
})
export class AppStatusStore {
  private readonly appStatusHttpService = inject(AppStatusHttpService);
  private readonly appStatusSseService = inject(AppStatusSseService);

  queryParameters = signal<AppStatusQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedId = signal<string | undefined>(undefined);
  
  created = signal<AppStatusResponse | null>(null);
  updated = signal<AppStatusResponse | null>(null);
  deleted = signal<AppStatusResponse | null>(null);

  private pagedListResource = rxResource({
    params: () => this.queryParameters(),
    stream: ({ params }) => this.appStatusHttpService.getPagedListByQueryAsync(params),
    defaultValue: [],
  });

  private selectedResource = rxResource({
    params: () => this.selectedId(),
    stream: ({ params }) => this.appStatusHttpService.getById(params),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(
    () => this.pagedListResource.isLoading() || this.selectedResource.isLoading()
  );
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());

  constructor() {
    this.appStatusSseService.connect({
      onCreated: (appStatus) => this.created.set(appStatus),
      onUpdated: (appStatus) => this.updated.set(appStatus),
      onDeleted: (appStatus) => this.deleted.set(appStatus),
    });
  }
}
