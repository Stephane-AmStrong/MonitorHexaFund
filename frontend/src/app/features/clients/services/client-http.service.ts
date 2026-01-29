import { computed, inject, Injectable, Signal } from '@angular/core';
import { BehaviorSubject, Observable, startWith, switchMap, tap } from 'rxjs';
import { ClientResponse } from '../models/client-response';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { ClientCreateRequest } from '../models/client-create-request';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { ClientQueryParameters } from '../models/client-query-parameters';
import { ClientDetailedResponse } from '../models/client-detailed-response';
import { httpResource } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ClientHttpService {
  private http = inject(BaseApiService);
  private refreshTrigger = new BehaviorSubject<void>(undefined);

  getPagedListResource(queryParams: Signal<ClientQueryParameters | undefined>) {
    return httpResource<ClientResponse[]>(() => {
      if (!queryParams()) return undefined;

      return {
        url: API_ENDPOINTS.CLIENTS.ROOT,
        params: computed(() => this.http.cleanQueryParams(queryParams()))(),
        defaultValue: [],
      };
    });    
  }

  getByLogin(login: Signal<string>) {
    return httpResource<ClientDetailedResponse>(() => {
      if (!login()) return undefined;

      return {
        url: API_ENDPOINTS.CLIENTS.BY_LOGIN(login()),
      };
    });
  }

  create(
    client: ClientCreateRequest
  ): Observable<ClientResponse> {
    return this.http
      .handleRequest<ClientResponse>('POST', API_ENDPOINTS.CLIENTS.ROOT, {
        body: client,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  update(id: string, changes: Partial<ClientCreateRequest>) {
    return this.http
      .handleRequest('PUT', API_ENDPOINTS.CLIENTS.BY_ID(id), {
        body: changes,
      })
      .pipe(tap(() => this.refreshTrigger.next()));
  }

  delete(id: string) {
    return this.http
      .handleRequest('DELETE', API_ENDPOINTS.CLIENTS.BY_ID(id))
      .pipe(tap(() => this.refreshTrigger.next()));
  }
}
