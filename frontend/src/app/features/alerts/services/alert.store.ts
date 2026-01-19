import { computed, inject, Injectable, signal } from '@angular/core';
import { AlertHttpService } from './alert-http.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { AlertQueryParameters } from '../models/alert-query-parameters';
import { AlertSseService } from './alert-sse.service';
import { AlertResponse } from '../models/alert-response';

@Injectable({
  providedIn: 'root',
})
export class AlertStore {
  private readonly alertHttpService = inject(AlertHttpService);
  private readonly alertSseService = inject(AlertSseService);

  queryParameters = signal<AlertQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedId = signal<string | undefined>(undefined);

  created = signal<AlertResponse | null>(null);
  updated = signal<AlertResponse | null>(null);
  deleted = signal<AlertResponse | null>(null);

  private pagedListResource = rxResource({
    params: () => this.queryParameters(),
    stream: ({ params }) => this.alertHttpService.getPagedListByQueryAsync(params),
    defaultValue: [],
  });

  private selectedResource = rxResource({
    params: () => this.selectedId(),
    stream: ({ params }) => this.alertHttpService.getById(params),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(
    () => this.pagedListResource.isLoading() || this.selectedResource.isLoading()
  );
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());

  constructor() {
    this.alertSseService.connect({
      onCreated: (alert) => this.created.set(alert),
      onUpdated: (alert) => this.updated.set(alert),
      onDeleted: (alert) => this.deleted.set(alert),
    });
  }
}
