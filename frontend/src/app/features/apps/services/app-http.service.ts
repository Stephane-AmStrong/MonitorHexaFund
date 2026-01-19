import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, startWith, switchMap, tap } from 'rxjs';
import { AppResponse } from '../models/app-response';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { AppCreateRequest } from '../models/app-create-request';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { AppQueryParameters } from '../models/app-query-parameters';
import { AppDetailedResponse } from '../models/app-detailed-response';

@Injectable({
  providedIn: 'root',
})
export class AppHttpService {
  private http = inject(BaseApiService);
  private refreshTrigger = new BehaviorSubject<void>(undefined);

  getPagedListByQueryAsync(queryParams: AppQueryParameters): Observable<AppResponse[]> {
    return this.refreshTrigger.pipe(
      startWith(undefined),
      switchMap(() =>
        this.http.handleRequest<AppResponse[]>('GET', API_ENDPOINTS.APPS.ROOT, { params: queryParams })
      )
    );
  }

  getById(id: string): Observable<AppDetailedResponse> {
    return this.refreshTrigger.pipe(
      startWith(undefined),
      switchMap(() =>
        this.http.handleRequest<AppDetailedResponse>(
          'GET', API_ENDPOINTS.APPS.BY_ID(id)
        )
      )
    );
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
