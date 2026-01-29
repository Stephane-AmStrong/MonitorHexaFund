import { computed, inject, Injectable, signal } from '@angular/core';
import { AppStatusHttpService } from './app-status-http.service';
import { AppStatusQueryParameters } from '../models/app-status-query-parameters';
import { AppStatusSseService } from './app-status-sse.service';
import { AppStatusResponse } from '../models/app-status-response';

@Injectable({
  providedIn: 'root',
})
export class AppStatusStore {
  private readonly appStatusHttpService = inject(AppStatusHttpService);
  private readonly appStatusSseService = inject(AppStatusSseService);

  queryParameters = signal<AppStatusQueryParameters | undefined>(undefined);
  selectedId = signal<string>('');

  created = signal<AppStatusResponse | null>(null);
  updated = signal<AppStatusResponse | null>(null);
  deleted = signal<AppStatusResponse | null>(null);

  private pagedListResource = this.appStatusHttpService.getPagedListResource(this.queryParameters);

  private selectedResource = this.appStatusHttpService.getById(this.selectedId);

  selected = this.selectedResource.value;
  pagedListIsLoading = this.pagedListResource.isLoading;
  selectedIsLoading = this.selectedResource.isLoading;
  pagedListError = this.pagedListResource.error;
  selectedError = this.selectedResource.error;
  pagedList = computed(() => this.pagedListResource.value() ?? []);
  pagingData = computed(() => JSON.parse(this.pagedListResource.headers()?.get('x-pagination') || '{}'));

  constructor() {
    this.appStatusSseService.connect({
      onCreated: (appStatus) => this.created.set(appStatus),
      onUpdated: (appStatus) => this.updated.set(appStatus),
      onDeleted: (appStatus) => this.deleted.set(appStatus),
    });
  }
}
