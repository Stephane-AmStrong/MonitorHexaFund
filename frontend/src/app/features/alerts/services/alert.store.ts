import { computed, inject, Injectable, signal } from '@angular/core';
import { AlertHttpService } from './alert-http.service';
import { AlertQueryParameters } from '../models/alert-query-parameters';
import { AlertSseService } from './alert-sse.service';
import { AlertResponse } from '../models/alert-response';

@Injectable({
  providedIn: 'root',
})
export class AlertStore {
  private readonly alertHttpService = inject(AlertHttpService);
  private readonly alertSseService = inject(AlertSseService);

  queryParameters = signal<AlertQueryParameters | undefined>(undefined);
  selectedId = signal<string>('');

  created = signal<AlertResponse | null>(null);
  updated = signal<AlertResponse | null>(null);
  deleted = signal<AlertResponse | null>(null);

  private pagedListResource = this.alertHttpService.getPagedListResource(this.queryParameters);

  private selectedResource = this.alertHttpService.getById(this.selectedId);

  selected = this.selectedResource.value;
  pagedListIsLoading = this.pagedListResource.isLoading;
  selectedIsLoading = this.selectedResource.isLoading;
  pagedListError = this.pagedListResource.error;
  selectedError = this.selectedResource.error;
  pagedList = computed(() => this.pagedListResource.value() ?? []);
  pagingData = computed(() => JSON.parse(this.pagedListResource.headers()?.get('x-pagination') || '{}'));

  constructor() {
    this.alertSseService.connect({
      onCreated: (alert) => this.created.set(alert),
      onUpdated: (alert) => this.updated.set(alert),
      onDeleted: (alert) => this.deleted.set(alert),
    });
  }
}
