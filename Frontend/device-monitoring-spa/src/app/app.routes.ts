// src/app/app.routes.ts
import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/device-list/device-list.component')
      .then(m => m.DeviceListComponent),
    title: 'Список устройств'
  },
  {
    path: 'device/:id',
    loadComponent: () => import('./features/device-list/device-details/device-detail.component')
      .then(m => m.DeviceDetailComponent),
    title: 'Детали устройства'
  },
  { path: '**', redirectTo: '' }
];