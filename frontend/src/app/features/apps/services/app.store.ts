import { computed, inject, Injectable, signal } from '@angular/core';
import { AppQueryParameters } from '../models/app-query-parameters';
import { AppHttpService } from './app-http.service';
import { AppSseService } from './app-sse.service';
import { AppResponse } from '../models/app-response';
import { HostHttpService } from '../../hosts/services/host-http.service';
import { PagingMetadata } from '../../../core/services/rest-api/paging/paging-metadata';

@Injectable({
  providedIn: 'root',
})
export class AppStore {
  private readonly appHttpService = inject(AppHttpService);
  private readonly hostHttpService = inject(HostHttpService);
  private readonly appSseService = inject(AppSseService);

  queryParameters = signal<AppQueryParameters | undefined>(undefined);
  selectedId = signal<[hostName: string, appName: string] | undefined>(undefined);
  
  created = signal<AppResponse | null>(null);
  updated = signal<AppResponse | null>(null);
  deleted = signal<AppResponse | null>(null);

  private pagedListResource = this.appHttpService.getPagedListResource(this.queryParameters);

  private selectedResource = this.hostHttpService.getAppByHostAndApp(
    computed(() => this.selectedId() ? this.selectedId()![0] : ''),
    computed(() => this.selectedId() ? this.selectedId()![1] : '')
  );

  selected = this.selectedResource.value;
  pagedListIsLoading = this.pagedListResource.isLoading;
  selectedIsLoading = this.selectedResource.isLoading;
  pagedListError = this.pagedListResource.error;
  selectedError = this.selectedResource.error;
  pagedList = computed(() => this.pagedListResource.value() ?? []);
  pagingData = computed(() => JSON.parse(this.pagedListResource.headers()?.get('x-pagination') || '{}') as PagingMetadata);

  constructor() {
    this.appSseService.connect({
      onCreated: (app) => this.created.set(app),
      onUpdated: (app) => this.updated.set(app),
      onDeleted: (app) => this.deleted.set(app),
    });
  }
}
