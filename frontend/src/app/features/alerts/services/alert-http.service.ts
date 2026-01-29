import { computed, inject, Injectable, Signal } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AlertResponse } from '../models/alert-response';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { AlertCreateOrIncrementRequest } from '../models/alert-create-or-increment-request';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { AlertQueryParameters } from '../models/alert-query-parameters';
import { AlertDetailedResponse } from '../models/alert-detailed-response';
import { httpResource } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AlertHttpService {
  private http = inject(BaseApiService);
  private refreshTrigger = new BehaviorSubject<void>(undefined);

  getPagedListResource(queryParams: Signal<AlertQueryParameters | undefined>) {
    return httpResource<AlertResponse[]>(() => {
      if (!queryParams()) return undefined;

      return {
        url: API_ENDPOINTS.ALERTS.ROOT,
        params: computed(() => this.http.cleanQueryParams(queryParams()))(),
        defaultValue: [],
      };
    });
  }

  getById(id: Signal<string>) {
    return httpResource<AlertDetailedResponse[]>(() => {
      if (!id()) return undefined;

      return {
        url: API_ENDPOINTS.ALERTS.BY_ID(id()),
      };
    });
  }

  create(
    alert: AlertCreateOrIncrementRequest
  ): Observable<AlertResponse> {
    return this.http
      .handleRequest<AlertResponse>('POST', API_ENDPOINTS.ALERTS.ROOT, {
        body: alert,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  update(id: string, changes: Partial<AlertCreateOrIncrementRequest>) {
    return this.http
      .handleRequest('PUT', API_ENDPOINTS.ALERTS.BY_ID(id), {
        body: changes,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  delete(id: string) {
    return this.http
      .handleRequest('DELETE', API_ENDPOINTS.ALERTS.BY_ID(id))
      .pipe(tap(() => this.refreshTrigger.next()));
  }
}
