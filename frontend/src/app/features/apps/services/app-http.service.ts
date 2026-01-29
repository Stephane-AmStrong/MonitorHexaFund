import { computed, inject, Injectable, Signal } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AppResponse } from '../models/app-response';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { AppCreateRequest } from '../models/app-create-request';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { AppQueryParameters } from '../models/app-query-parameters';
import { httpResource } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AppHttpService {
  private http = inject(BaseApiService);
  private refreshTrigger = new BehaviorSubject<void>(undefined);

  getPagedListResource(queryParams: Signal<AppQueryParameters | undefined>) {
    return httpResource<AppResponse[]>(() => {
      if (!queryParams()) return undefined;

      return {
        url: API_ENDPOINTS.APPS.ROOT,
        params: computed(() => this.http.cleanQueryParams(queryParams()))(),
        defaultValue: [],
      };
    });
  }

  getById(id: Signal<string>) {
    return httpResource<AppResponse[]>(() => {
      if (!id()) return undefined;

      return {
        url: API_ENDPOINTS.APPS.BY_ID(id()),
      };
    });
  }

  create(
    app: AppCreateRequest
  ): Observable<AppResponse> {
    return this.http
      .handleRequest<AppResponse>('POST', API_ENDPOINTS.APPS.ROOT, {
        body: app,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  update(id: string, changes: Partial<AppCreateRequest>) {
    return this.http
      .handleRequest('PUT', API_ENDPOINTS.APPS.BY_ID(id), {
        body: changes,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  delete(id: string) {
    return this.http
      .handleRequest('DELETE', API_ENDPOINTS.APPS.BY_ID(id))
      .pipe(tap(() => this.refreshTrigger.next()));
  }

}
