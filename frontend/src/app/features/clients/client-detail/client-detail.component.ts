import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { ClientStore } from '../services/client.store';
import { ConnectionTableComponent } from '../../connections/connection-table/connection-table.component';
import { DetailCardComponent } from '../../../shared/components/detail-card/detail-card.component';
import { ConnectionStore } from '../../connections/services/connection.store';
import { ConnectionQueryParameters } from '../../connections/models/connection-query-parameters';

@Component({
  selector: 'client-detail',
  imports: [
    ToolbarComponent,
    MatTabsModule,
    MatCardModule,
    MatIconModule,
    MatGridListModule,
    MatDividerModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatListModule,
    ConnectionTableComponent,
    DetailCardComponent,
  ],
  templateUrl: './client-detail.component.html',
  styleUrl: './client-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ClientDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly clientStore = inject(ClientStore);
  private readonly connectionStore = inject(ConnectionStore);

  private readonly routeResource = toSignal(this.route.params);

  private readonly selectedLogin = computed<string>(() => this.routeResource()?.['login']);
  private readonly connectionQueryParams = computed<ConnectionQueryParameters>(() => ({ withClientGaia: this.clientDetail()?.id}));

  readonly isLoading = this.clientStore.isLoading;
  readonly clientDetail = this.clientStore.selected;
  readonly connections = this.connectionStore.pagedList;


  constructor() {
    effect(() => this.clientStore.selectedLogin.set(this.selectedLogin()));
    effect(() => this.connectionStore.queryParameters.set(this.connectionQueryParams()));
  }
}
