import { inject, Injectable } from '@angular/core';
import { RxSseService } from '../../../core/services/sse/rx-sse.service';
import { AppStatusResponse } from '../models/app-status-response';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { SseEventHandlers } from '../../../core/services/sse/sse-event-handlers';

@Injectable({
  providedIn: 'root',
})
export class AppStatusSseService {
  private rxSseService = inject(RxSseService);

  connect(handlers: SseEventHandlers<AppStatusResponse>): void {
    return this.rxSseService.connect<AppStatusResponse>(
      API_ENDPOINTS.APP_STATUSES.LIVE(),
      handlers
    );
  }
}
