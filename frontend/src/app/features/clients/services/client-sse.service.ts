import { inject, Injectable } from '@angular/core';
import { RxSseService } from '../../../core/services/sse/rx-sse.service';
import { ClientResponse } from '../models/client-response';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { SseEventHandlers } from '../../../core/services/sse/sse-event-handlers';

@Injectable({
  providedIn: 'root',
})
export class ClientSseService {
  private rxSseService = inject(RxSseService);

  connect(handlers: SseEventHandlers<ClientResponse>): void {
    return this.rxSseService.connect<ClientResponse>(API_ENDPOINTS.CLIENTS.LIVE(), handlers);
  }
}
