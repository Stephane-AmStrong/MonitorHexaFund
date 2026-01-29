import { Component, effect, inject, model, viewChild } from '@angular/core';
import { ClientResponse } from '../models/client-response';
import { Router } from '@angular/router';
import { MatTable, MatTableModule } from '@angular/material/table';
import { ClientStore } from '../services/client.store';
import {MatSort, MatSortModule, Sort} from '@angular/material/sort';

@Component({
  selector: 'client-table',
  imports: [MatTableModule, MatSortModule],
  templateUrl: './client-table.component.html',
  styleUrl: './client-table.component.scss'
})
export class ClientTableComponent {
  private readonly router = inject(Router);
  private readonly clientStore = inject(ClientStore);
  clients = model.required<ClientResponse[]>();
  readonly table = viewChild.required<MatTable<ClientResponse>>(MatTable);
  readonly sort = viewChild.required<MatSort>(MatSort);

  readonly clientColumns: readonly string[] = ['gaia', 'login'] as const;


  constructor() {
    effect(() => {
      const createdClient = this.clientStore.created();
      if (!createdClient) return;

      this.clients.update(clients => [...clients, createdClient]);
      this.table().renderRows();
    });

    effect(() => {
      const updatedClient = this.clientStore.updated();
      if (!updatedClient) return;

      this.clients.update(clients => clients.map(client => client.id === updatedClient.id ? updatedClient : client));
      this.table().renderRows();
    });

    effect(() => {
      const deletedClient = this.clientStore.deleted();
      if (!deletedClient) return;

      this.clients.update(clients => clients.filter(client => client.id !== deletedClient.id));
      this.table().renderRows();
    });
  }

  onClientClicked(client: ClientResponse) {
    const clientLogin = client.login;
    if (clientLogin) {
      this.router.navigate(['/clients', 'login', clientLogin]);
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
