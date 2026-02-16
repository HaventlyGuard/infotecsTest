import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DeleteParameters, DeviceInfoPage, SessionInfoPage, SortDirection } from '../models/device.models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'http://localhost:5000/api/v1';
  
  constructor(private http: HttpClient) { }
  
  getDevices(offset: number, limit: number, sortDirection: SortDirection): Observable<DeviceInfoPage> {
    const params = new HttpParams()
      .set('limit', limit)
      .set('offset', offset)
      .set('sortDirection', sortDirection);
    return this.http.get<DeviceInfoPage>(`${this.apiUrl}/devices`, { params });
  }
  
  getSessions(deviceId: string, offset: number, limit: number, sortDirection: SortDirection): Observable<SessionInfoPage> {
    const params = new HttpParams()
      .set('offset', offset)
      .set('limit', limit)
      .set('sortDirection', sortDirection);

    return this.http.get<SessionInfoPage>(`${this.apiUrl}/sessions/${deviceId}`, { params });
  }
  
  deleteSessions(deleteParams: DeleteParameters): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/sessions`, {
      body: deleteParams
    });
  }
}