import { inject, Injectable } from '@angular/core';
import { RxSseService } from '../../../core/services/sse/rx-sse.service';
import { ConnectionResponse } from '../models/connection-response';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { SseEventHandlers } from '../../../core/services/sse/sse-event-handlers';

@Injectable({
  providedIn: 'root',
})
export class ConnectionSseService {
  private rxSseService = inject(RxSseService);

  connect(handlers: SseEventHandlers<ConnectionResponse>): void {
    return this.rxSseService.connect<ConnectionResponse>(
      API_ENDPOINTS.CONNECTIONS.LIVE(),
      handlers
    );
  }
}
