import { inject, Injectable } from '@angular/core';
import { RxSseService } from '../../../core/services/sse/rx-sse.service';
import { AlertResponse } from '../models/alert-response';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { SseEventHandlers } from '../../../core/services/sse/sse-event-handlers';

@Injectable({
  providedIn: 'root',
})
export class AlertSseService {
  private rxSseService = inject(RxSseService);

  connect(handlers: SseEventHandlers<AlertResponse>): void {
    return this.rxSseService.connect<AlertResponse>(API_ENDPOINTS.ALERTS.LIVE(), handlers);
  }
}
