import { Routes } from '@angular/router';
import { MainLayoutComponent } from './core/layouts/main-layout/main-layout.component';
import { TagListComponent } from './features/tags/pages/tag-list/tag-list.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: 'tags', component: TagListComponent },
      { path: '**', redirectTo: 'tags' },
    ],
  },
];
