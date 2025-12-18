import { computed, inject, Injectable, signal } from '@angular/core';
import { HostService } from './host.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { HostQueryParameters } from '../models/host-query-parameters';

@Injectable({
  providedIn: 'root',
})
export class HostStore {
  private hostService = inject(HostService);

  queryParameters = signal<HostQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedName = signal<string | undefined>(undefined);

  private pagedListResource = rxResource({
    params: () => (this.queryParameters()),
    stream: ({params}) => this.hostService.getPagedListByQueryAsync(params),
    defaultValue: [],
  })

  private selectedResource = rxResource({
    params: () => (this.selectedName()),
    stream: ({params}) => this.hostService.getByName(params),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(() => this.pagedListResource.isLoading() || this.selectedResource.isLoading());
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());
}
