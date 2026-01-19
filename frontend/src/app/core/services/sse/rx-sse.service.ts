import { Injectable, OnDestroy } from '@angular/core';
import {
  BehaviorSubject,
  Observable,
  retry,
  Subject,
  Subscription,
  takeUntil,
  timer,
} from 'rxjs';
import { BaseModel } from '../../models/base-model';
import { environment } from '../../../../environments/environment.development';
import { SseEventHandlers } from './sse-event-handlers';
import { SseEventModel } from '../../models/sse-event-model';
import { SseConnectionState } from '../../models/sse-message-enums';

@Injectable({
  providedIn: 'root',
})
export class RxSseService implements OnDestroy {
  private readonly RECONNECT_DELAY_MS = 1500;
  private readonly connectionStateSubject = new BehaviorSubject<SseConnectionState>('Closed');

  private connectionSub?: Subscription;

  private readonly destroy$ = new Subject<void>();

  connect<T extends BaseModel>(sseEndpoint: string, handlers: SseEventHandlers<T>): void {
    this.disconnect();
    
    const streamingUrl = `${environment.apiUrl}/${sseEndpoint}`;

    this.connectionSub = this.createStream<T>(streamingUrl)
      .pipe(
        retry({
          delay: (error, retryCount) => {
            const delayMs = this.computeBackoffDelay(retryCount);

            console.warn(
              `SSE disconnected — retry #${retryCount} in ${delayMs}ms`,
              error
            );

            this.connectionStateSubject.next('Connecting');
            return timer(delayMs);
          },
        }),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: event => this.dispatch(event, handlers),
        error: err => {
          console.error('SSE fatal error', err);
          this.connectionStateSubject.next('Error');
        },
      });
  }

  disconnect(): void {
    this.connectionSub?.unsubscribe();
    this.connectionSub = undefined;
    this.connectionStateSubject.next('Closed');
  }

  private createStream<T extends BaseModel>(url: string): Observable<SseEventModel<T>> {
    return new Observable<SseEventModel<T>>((observer) => {
      const eventSource = new EventSource(url);

      this.connectionStateSubject.next('Connecting');

      eventSource.onopen = () => {
        console.log('SSE connected');
        this.connectionStateSubject.next('Open');
      };

      eventSource.onmessage = (event) => {
        try {
          observer.next(JSON.parse(event.data));
        } catch (e) {
          observer.error(e);
        }
      };

      eventSource.onerror = (error) => {
        console.warn('SSE error', error);
        eventSource.close();
        this.connectionStateSubject.next('Closed');
        observer.error(error);
      };

      return () => {
        eventSource.close();
        this.connectionStateSubject.next('Closed');
      };
    });
  }

  private dispatch<T extends BaseModel>(
    sse: SseEventModel<T>,
    handlers: SseEventHandlers<T>
  ): void {
    switch (sse.event) {
      case 'Created':
        handlers.onCreated?.(sse.content);
        break;

      case 'Updated':
        handlers.onUpdated?.(sse.content);
        break;

      case 'Deleted':
        handlers.onDeleted?.(sse.content);
        break;
    }
  }

  private computeBackoffDelay(retryCount: number): number {
    const exponentialDelay = this.RECONNECT_DELAY_MS * Math.pow(2, retryCount - 1);

    return Math.min(exponentialDelay, this.RECONNECT_DELAY_MS * 5);
  }

  
  ngOnDestroy(): void {
    console.debug('RxSseService destroyed');
    this.destroy$.next();
    this.destroy$.complete();
    this.disconnect();
  }
}
