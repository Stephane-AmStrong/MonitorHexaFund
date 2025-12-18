import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatGridListModule } from '@angular/material/grid-list';
import { ActivatedRoute, Router } from '@angular/router';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';

import { ServerResponse } from '../../servers/models/server-response';
import { MatTableModule } from '@angular/material/table';
import { toSignal } from '@angular/core/rxjs-interop';
import { HostStore } from '../services/host.store';
import { MatCardModule } from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { ServerTableComponent } from "../../servers/server-table/server-table.component";

@Component({
  selector: 'host-detail',
  imports: [MatCardModule, MatGridListModule, MatIcon, MatTableModule, MatButtonModule, ToolbarComponent, ServerTableComponent],
  templateUrl: './host-detail.component.html',
  styleUrl: './host-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HostDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly hostStore = inject(HostStore);

  constructor() {
    effect(() => this.hostStore.selectedName.set(this.selectedName()));
  }

  private readonly routeResource = toSignal(this.route.params);

  private readonly selectedName = computed<string>(() => (this.routeResource()?.['hostName']));

  readonly isLoading = this.hostStore.isLoading;
  readonly hostDetail = this.hostStore.selected;

  readonly servers = computed<ServerResponse[]>(() => this.hostDetail()?.servers || []);

  private dialog = inject(MatDialog);

  serverColumns: string[] = ['appName', 'version', 'port', 'type', 'cronStartTime', 'cronStopTime'] as const;

  onRowClicked(row: ServerResponse) {
    const hostName = row.hostName;
    const appName = row.appName;
    if (hostName && appName) {
      this.router.navigate(['/hosts', hostName, 'servers', appName]);
    }
  }
}
