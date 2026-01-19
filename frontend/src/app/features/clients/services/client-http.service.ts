import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, startWith, switchMap, tap } from 'rxjs';
import { ClientResponse } from '../models/client-response';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { ClientCreateRequest } from '../models/client-create-request';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { ClientQueryParameters } from '../models/client-query-parameters';
import { ClientDetailedResponse } from '../models/client-detailed-response';

@Injectable({
  providedIn: 'root',
})
export class ClientHttpService {
  private http = inject(BaseApiService);
  private refreshTrigger = new BehaviorSubject<void>(undefined);

  getPagedListByQueryAsync(queryParams: ClientQueryParameters): Observable<ClientResponse[]> {
    return this.refreshTrigger.pipe(
      startWith(undefined),
      switchMap(() =>
        this.http.handleRequest<ClientResponse[]>('GET', API_ENDPOINTS.CLIENTS.ROOT, { params: queryParams })
      )
    );
  }

  getByLogin(login: string): Observable<ClientDetailedResponse> {
    return this.refreshTrigger.pipe(
      startWith(undefined),
      switchMap(() =>
        this.http.handleRequest<ClientDetailedResponse>(
          'GET', API_ENDPOINTS.CLIENTS.BY_LOGIN(login)
        )
      )
    );
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
