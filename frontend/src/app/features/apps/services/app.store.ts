import { computed, inject, Injectable, signal } from '@angular/core';
import { AppQueryParameters } from '../models/app-query-parameters';
import { AppHttpService } from './app-http.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { HostHttpService } from '../../hosts/services/host-http.service';
import { AppSseService } from './app-sse.service';
import { AppResponse } from '../models/app-response';

@Injectable({
  providedIn: 'root',
})
export class AppStore {
  private readonly appHttpService = inject(AppHttpService);
  private readonly appSseService = inject(AppSseService);
  private readonly hostHttpService = inject(HostHttpService);

  queryParameters = signal<AppQueryParameters | undefined>({
    page: 1,
    pageSize: 10,
  });

  selectedId = signal<[hostName: string, appName: string] | undefined>(undefined);
  
  created = signal<AppResponse | null>(null);
  updated = signal<AppResponse | null>(null);
  deleted = signal<AppResponse | null>(null);

  private pagedListResource = rxResource({
    params: () => this.queryParameters(),
    stream: ({ params }) => this.appHttpService.getPagedListByQueryAsync(params),
    defaultValue: [],
  });

  private selectedResource = rxResource({
    params: () => this.selectedId(),
    stream: ({ params: [hostName, appName] }) =>
      this.hostHttpService.getAppByHostAndApp(hostName, appName),
    defaultValue: undefined,
  });

  selected = this.selectedResource.value;
  pagedList = this.pagedListResource.value;
  isLoading = computed(
    () => this.pagedListResource.isLoading() || this.selectedResource.isLoading()
  );
  error = computed(() => this.pagedListResource.error() || this.selectedResource.error());

  constructor() {
    this.appSseService.connect({
      onCreated: (app) => this.created.set(app),
      onUpdated: (app) => this.updated.set(app),
      onDeleted: (app) => this.deleted.set(app),
    });
  }
}
