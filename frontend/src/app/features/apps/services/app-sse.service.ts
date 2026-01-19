import { inject, Injectable } from '@angular/core';
import { RxSseService } from '../../../core/services/sse/rx-sse.service';
import { AppResponse } from '../models/app-response';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { SseEventHandlers } from '../../../core/services/sse/sse-event-handlers';

@Injectable({
  providedIn: 'root',
})
export class AppSseService {
  private rxSseService = inject(RxSseService);

  connect(handlers: SseEventHandlers<AppResponse>): void {
    return this.rxSseService.connect<AppResponse>(API_ENDPOINTS.APPS.LIVE(), handlers);
  }
}
