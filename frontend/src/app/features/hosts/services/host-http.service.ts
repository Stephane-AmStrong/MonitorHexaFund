import { computed, inject, Injectable, Signal } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { HostResponse } from '../models/host-response';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { HostCreateRequest } from '../models/host-create-request';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { HostDetailedResponse } from '../models/host-detailed-response';
import { HostQueryParameters } from '../models/host-query-parameters';
import { AppDetailedResponse } from '../../apps/models/app-detailed-response';
import { httpResource } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class HostHttpService {
  private http = inject(BaseApiService);
  private refreshTrigger = new BehaviorSubject<void>(undefined);

  getPagedListResource(queryParams: Signal<HostQueryParameters | undefined>) {
    return httpResource<HostDetailedResponse[]>(() => {
      if (!queryParams()) return undefined;

      return {
        url: API_ENDPOINTS.HOSTS.ROOT,
        params: computed(() => this.http.cleanQueryParams(queryParams()))(),
        defaultValue: [],
      };
    });
  }

  getByName(name: Signal<string>) {
    return httpResource<HostDetailedResponse>(() => {
      if (!name()) return undefined;

      return {
        url: API_ENDPOINTS.HOSTS.BY_NAME(name()),
      };
    });
  }

  getAppByHostAndApp(hostName: Signal<string>, appName: Signal<string>) {
    return httpResource<AppDetailedResponse>(() => {
      if (!hostName() || !appName()) return undefined;

      return {
        url: API_ENDPOINTS.HOSTS.APP_BY_HOST(hostName(), appName()),
      };
    });
  }

  create(
    host: HostCreateRequest
  ): Observable<HostResponse> {
    return this.http
      .handleRequest<HostResponse>('POST', API_ENDPOINTS.HOSTS.ROOT, {
        body: host,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  update(id: string, changes: Partial<HostCreateRequest>) {
    return this.http
      .handleRequest('PUT', API_ENDPOINTS.HOSTS.BY_ID(id), {
        body: changes,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  delete(id: string) {
    return this.http
      .handleRequest('DELETE', API_ENDPOINTS.HOSTS.BY_ID(id))
      .pipe(tap(() => this.refreshTrigger.next()));
  }
}
