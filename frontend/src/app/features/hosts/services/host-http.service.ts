import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, startWith, switchMap, tap } from 'rxjs';
import { HostResponse } from '../models/host-response';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { HostCreateRequest } from '../models/host-create-request';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { HostDetailedResponse } from '../models/host-detailed-response';
import { HostQueryParameters } from '../models/host-query-parameters';
import { AppDetailedResponse } from '../../apps/models/app-detailed-response';

@Injectable({
  providedIn: 'root',
})
export class HostHttpService {
  private http = inject(BaseApiService);
  private refreshTrigger = new BehaviorSubject<void>(undefined);

  getPagedListByQueryAsync(queryParams: HostQueryParameters): Observable<HostDetailedResponse[]> {
    return this.refreshTrigger.pipe(
      startWith(undefined),
      switchMap(() =>
        this.http.handleRequest<HostDetailedResponse[]>('GET', API_ENDPOINTS.HOSTS.WITH_APPS(), { params: queryParams })
      )
    );
  }

  getByName(name: string): Observable<HostDetailedResponse> {
    return this.refreshTrigger.pipe(
      startWith(undefined),
      switchMap(() =>
        this.http.handleRequest<HostDetailedResponse>(
          'GET',API_ENDPOINTS.HOSTS.BY_NAME(name)
        )
      )
    );
  }

  getAppByHostAndApp(hostName: string, appName: string): Observable<AppDetailedResponse> {
    return this.refreshTrigger.pipe(
      startWith(undefined),
      switchMap(() =>
        this.http.handleRequest<AppDetailedResponse>(
          'GET', API_ENDPOINTS.HOSTS.APP_BY_HOST(hostName, appName)
        )
      )
    );
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
