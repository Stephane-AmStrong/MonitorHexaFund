import { inject, Injectable } from '@angular/core';
import { RxSseService } from '../../../core/services/sse/rx-sse.service';
import { HostResponse } from '../models/host-response';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints';
import { SseEventHandlers } from '../../../core/services/sse/sse-event-handlers';

@Injectable({
  providedIn: 'root',
})
export class HostSseService {
  private rxSseService = inject(RxSseService);

  connect(handlers: SseEventHandlers<HostResponse>): void {
    return this.rxSseService.connect<HostResponse>(API_ENDPOINTS.HOSTS.LIVE(), handlers);
  }
}
