import { Component, effect, inject, model, viewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ConnectionResponse } from '../models/connection-response';
import { MatTable, MatTableModule } from '@angular/material/table';
import { PrefixPictogramComponent } from "../../../shared/components/prefix-pictogram/prefix-pictogram.component";
import { HashColorPipe } from "../../../shared/pipes/hash-color.pipe";
import { ConnectionStore } from '../services/connection.store';
import { DatePipe } from '@angular/common';
import { MatSort, MatSortModule, Sort } from '@angular/material/sort';

@Component({
  selector: 'connection-table',
  imports: [MatTableModule, PrefixPictogramComponent, HashColorPipe, DatePipe, MatSortModule],
  templateUrl: './connection-table.component.html',
  styleUrl: './connection-table.component.scss'
})
export class ConnectionTableComponent {
  private readonly router = inject(Router);
  private readonly connectionStore = inject(ConnectionStore);
  connections = model.required<ConnectionResponse[]>();

  readonly table = viewChild.required<MatTable<ConnectionResponse>>(MatTable);
  readonly sort = viewChild.required<MatSort>(MatSort);

  constructor() {
    effect(() => {
      const createdConnection = this.connectionStore.created();
      if (!createdConnection) return;

      this.connections.update(connections => [...connections, createdConnection]);
      this.table().renderRows();
    });

    effect(() => {
      const updatedConnection = this.connectionStore.updated();
      if (!updatedConnection) return;

      this.connections.update(connections => connections.map(connection => connection.id === updatedConnection.id ? updatedConnection : connection));
      this.table().renderRows();
    });

    effect(() => {
      const deletedConnection = this.connectionStore.deleted();
      if (!deletedConnection) return;

      this.connections.update(connections => connections.filter(connection => connection.id !== deletedConnection.id));
      this.table().renderRows();
    });
  }

  readonly connectionColumns: readonly string[] = ['clientGaia', 'appId', 'apiVersion', 'machine', 'processId', 'establishedAt', 'terminatedAt'] as const;

  onConnectionClicked(connection: ConnectionResponse) {
    const connectionId = connection.id;
    if (connectionId) {
      this.router.navigate(['/connections', connectionId]);
    }
  }

  onSortChange($event: Sort) {
     if ($event.active && $event.direction) {
      this.router.navigate([], {
        queryParams: {
          orderBy: `${$event.active} ${$event.direction}`,
        },
        queryParamsHandling: 'merge',
      });
    }
  }
}
