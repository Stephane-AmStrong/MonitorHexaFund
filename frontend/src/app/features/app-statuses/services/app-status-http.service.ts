import { computed, inject, Injectable, Signal } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { AppStatusQueryParameters } from '../models/app-status-query-parameters';
import { AppStatusCreateRequest } from '../models/app-status-create-request';
import { AppStatusDetailedResponse } from '../models/app-status-detailed-response';
import { AppStatusResponse } from '../models/app-status-response';
import { httpResource } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AppStatusHttpService {
  private http = inject(BaseApiService);
  private refreshTrigger = new BehaviorSubject<void>(undefined);

  getPagedListResource(queryParams: Signal<AppStatusQueryParameters | undefined>) {
    return httpResource<AppStatusResponse[]>(() => {
      if (!queryParams()) return undefined;

      return {
        url: API_ENDPOINTS.APP_STATUSES.ROOT,
        params: computed(() => this.http.cleanQueryParams(queryParams()))(),
        defaultValue: [],
      };
    });    
  }

  getById(id: Signal<string>) {
    return httpResource<AppStatusDetailedResponse[]>(() => {
      if (!id()) return undefined;

      return {
        url: API_ENDPOINTS.APP_STATUSES.BY_ID(id()),
      };
    });
  }

  create(
    appStatus: AppStatusCreateRequest
  ): Observable<AppStatusResponse> {
    return this.http
      .handleRequest<AppStatusResponse>('POST', API_ENDPOINTS.APP_STATUSES.ROOT, {
        body: appStatus,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  update(id: string, changes: Partial<AppStatusCreateRequest>) {
    return this.http
      .handleRequest('PUT', API_ENDPOINTS.APP_STATUSES.BY_ID(id), {
        body: changes,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  delete(id: string) {
    return this.http
      .handleRequest('DELETE', API_ENDPOINTS.APP_STATUSES.BY_ID(id))
      .pipe(tap(() => this.refreshTrigger.next()));
  }
}
