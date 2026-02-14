// src/app/services/device.service.ts
import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { toSignal, toObservable } from '@angular/core/rxjs-interop';
import { DeviceInfo, DeviceSession } from '../models/device.models';
import { switchMap, map } from 'rxjs/operators';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api/v1';

  selectedDeviceId = signal<string | null>(null);

  getDevices(): Observable<DeviceInfo[]> {
    console.log('Запрос к:', `${this.apiUrl}/devices`);
    return this.http.get<DeviceInfo[]>(`${this.apiUrl}/devices`);
  }

  getDeviceSessions(deviceId: string): Observable<DeviceSession[]> {
    console.log('Запрос к:', `${this.apiUrl}/devices/${deviceId}/sessions`);
    return this.http.get<DeviceSession[]>(`${this.apiUrl}/devices/${deviceId}/sessions`);
  }

  deleteOldSessions(deviceId: string, beforeDate: string): Observable<any> {
     console.log('Удаление:', `${this.apiUrl}/devices/${deviceId}/sessions?before=${beforeDate}`);
    return this.http.delete(`${this.apiUrl}/devices/${deviceId}/sessions`, {
      params: { before: beforeDate }
    });
  }

  readonly sessions$ = toObservable(this.selectedDeviceId).pipe(
    switchMap(deviceId => {
      if (!deviceId) return of([]);
      return this.getDeviceSessions(deviceId);
    })
  );
}