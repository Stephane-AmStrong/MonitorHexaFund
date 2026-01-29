import { computed, inject, Injectable, Signal } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ConnectionResponse } from '../models/connection-response';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { ConnectionQueryParameters } from '../models/connection-query-parameters';
import { ConnectionDetailedResponse } from '../models/connection-detailed-response';
import { ConnectionEstablishRequest } from '../models/connection-establish-request';
import { httpResource } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ConnectionHttpService {
  private http = inject(BaseApiService);
  private refreshTrigger = new BehaviorSubject<void>(undefined);

  getPagedListResource(queryParams: Signal<ConnectionQueryParameters | undefined>) {
    return httpResource<ConnectionResponse[]>(() => {
      if (!queryParams()) return undefined;

      return {
        url: API_ENDPOINTS.CONNECTIONS.ROOT,
        params: computed(() => this.http.cleanQueryParams(queryParams()))(),
        defaultValue: [],
      };
    });
  }

  getById(id: Signal<string>) {
    return httpResource<ConnectionDetailedResponse>(() => {
      if (!id()) return undefined;

      return {
        url: API_ENDPOINTS.CONNECTIONS.BY_ID(id()),
      };
    });
  }

  create(
    connection: ConnectionEstablishRequest
  ): Observable<ConnectionResponse> {
    return this.http
      .handleRequest<ConnectionResponse>('POST', API_ENDPOINTS.CONNECTIONS.ROOT, {
        body: connection,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  update(id: string, changes: Partial<ConnectionEstablishRequest>) {
    return this.http
      .handleRequest('PUT', API_ENDPOINTS.CONNECTIONS.BY_ID(id), {
        body: changes,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  delete(id: string) {
    return this.http
      .handleRequest('DELETE', API_ENDPOINTS.CONNECTIONS.BY_ID(id))
      .pipe(tap(() => this.refreshTrigger.next()));
  }
}
