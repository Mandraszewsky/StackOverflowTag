import { Component, inject, viewChild } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, MatSort, Sort } from '@angular/material/sort';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TagStore } from '../../tag.store';

@Component({
  selector: 'app-tag-list',
  standalone: true,
  imports: [
    DecimalPipe,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './tag-list.component.html',
  styleUrl: './tag-list.component.scss',
})
export class TagListComponent {
  protected readonly store = inject(TagStore);
  protected readonly displayedColumns = ['name', 'count', 'percentage'];

  protected onSortChange(sort: Sort): void {
    const direction = sort.direction || 'asc';
    this.store.changeSort(sort.active, direction as 'asc' | 'desc');
  }

  protected onPageChange(event: PageEvent): void {
    this.store.changePage(event.pageIndex + 1, event.pageSize);
  }

  protected onRefresh(): void {
    this.store.refreshTags();
  }
}
