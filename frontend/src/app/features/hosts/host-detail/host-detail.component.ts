import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatGridListModule } from '@angular/material/grid-list';
import { ActivatedRoute, Router } from '@angular/router';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';

import { AppResponse } from '../../apps/models/app-response';
import { MatTableModule } from '@angular/material/table';
import { toSignal } from '@angular/core/rxjs-interop';
import { HostStore } from '../services/host.store';
import { MatCardModule } from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { AppTableComponent } from '../../apps/app-table/app-table.component';
import { AppStore } from '../../apps/services/app.store';
import { AppQueryParameters } from '../../apps/models/app-query-parameters';

@Component({
  selector: 'host-detail',
  imports: [
    MatCardModule,
    MatGridListModule,
    MatIcon,
    MatTableModule,
    MatButtonModule,
    ToolbarComponent,
    AppTableComponent,
  ],
  templateUrl: './host-detail.component.html',
  styleUrl: './host-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HostDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly hostStore = inject(HostStore);
  private readonly appStore = inject(AppStore);

  private readonly routeResource = toSignal(this.route.params);

  private readonly selectedName = computed<string>(() => this.routeResource()?.['hostName']);

  private readonly appQueryParams = computed<AppQueryParameters>(() => ({
    withHostName: this.hostDetail()?.name,
  }));

  readonly selectedIsLoading = this.hostStore.selectedIsLoading;
  readonly hostDetail = this.hostStore.selected;
  readonly apps = this.appStore.pagedList;

  private dialog = inject(MatDialog);

  appColumns: string[] = [
    'appName',
    'version',
    'port',
    'type',
    'cronStartTime',
    'cronStopTime',
  ] as const;

  onRowClicked(row: AppResponse) {
    const hostName = row.hostName;
    const appName = row.appName;
    if (hostName && appName) {
      this.router.navigate(['/hosts', hostName, 'apps', appName]);
    }
  }

  constructor() {
    effect(() => this.hostStore.selectedName.set(this.selectedName()));
    effect(() => this.appStore.queryParameters.set(this.appQueryParams()));
  }
}
