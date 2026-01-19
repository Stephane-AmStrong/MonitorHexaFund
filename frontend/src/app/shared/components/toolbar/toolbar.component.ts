import { Component, inject, input, model } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { SidenavService } from '../../layouts/main-layout/sidenav.service';
import { BreadcrumbComponent } from '../breadcrumb/breadcrumb.component';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'toolbar',
  imports: [
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    BreadcrumbComponent,
  ],
  templateUrl: './toolbar.component.html',
  styleUrl: './toolbar.component.scss',
})
export class ToolbarComponent {
  protected sidenavService = inject(SidenavService);
  private readonly router = inject(Router);
  readonly environmentSuffix = environment.environmentSuffix;

  searchConfig = model<SearchConfig>({ showSearch: false });

  onSearch() {
    this.router.navigate([], {
      queryParams: {
        searchTerm: this.searchConfig().showSearch ? this.searchConfig().searchTerm : undefined,
      },
      queryParamsHandling: 'merge',
    })
  }

  toggleSidenav() {
    this.sidenavService.toggle();
  }
}
