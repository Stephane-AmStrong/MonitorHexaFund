import { computed, inject, Injectable, signal } from '@angular/core';
import { AlertService } from './alert.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { AlertQueryParameters } from '../models/alert-query-parameters';

@Injectable({
  providedIn: 'root',
})
export class AlertStore {
  private alertService = inject(AlertService);

  queryParameters = signal<AlertQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedId = signal<string | undefined>(undefined);

  private pagedListResource = rxResource({
    params: () => (this.queryParameters()),
    stream: ({params}) => this.alertService.getPagedListByQueryAsync(params),
    defaultValue: [],
  })

  private selectedResource = rxResource({
    params: () => (this.selectedId()),
    stream: ({params}) => this.alertService.getById(params),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(() => this.pagedListResource.isLoading() || this.selectedResource.isLoading());
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());
}
