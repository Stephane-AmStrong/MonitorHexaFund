import { Component, inject, input } from '@angular/core';
import { ServerStatusResponse } from '../models/server-status-response';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { ServerStatusIndicatorComponent } from "../../servers/server-status-indicator/server-status-indicator.component";

@Component({
  selector: 'server-status-table',
  imports: [MatTableModule, ServerStatusIndicatorComponent],
  templateUrl: './server-status-table.component.html',
  styleUrl: './server-status-table.component.scss'
})
export class ServerStatusTableComponent {
  private readonly router = inject(Router);
  serverStatuses = input.required<ServerStatusResponse[]>();

  readonly serverStatusColumns = input<string[]>(['server', 'recordedAt']);

  onServerStatusClicked(serverStatus: ServerStatusResponse) {
    const splitedServerId = serverStatus?.serverId?.split(';');

    const hostName = splitedServerId[0];
    const appName = splitedServerId[1];

    if (hostName && appName) {
      this.router.navigate(['/hosts', hostName, 'servers', appName]);
    }
  }
}
