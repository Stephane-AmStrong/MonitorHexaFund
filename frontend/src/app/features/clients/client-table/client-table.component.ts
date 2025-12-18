import { Component, inject, input } from '@angular/core';
import { ClientResponse } from '../models/client-response';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'client-table',
  imports: [MatTableModule],
  templateUrl: './client-table.component.html',
  styleUrl: './client-table.component.scss'
})
export class ClientTableComponent {
  private readonly router = inject(Router);
  clients = input.required<ClientResponse[]>();
  
  readonly clientColumns: readonly string[] = ['gaia', 'login'] as const;

  onClientClicked(client: ClientResponse) {
    const clientLogin = client.login;
    if (clientLogin) {
      this.router.navigate(['/clients', 'login', clientLogin]);
    }
  }

}
