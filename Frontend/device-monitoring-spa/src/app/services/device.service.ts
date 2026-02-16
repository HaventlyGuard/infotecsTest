import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DeleteParameters, DeviceInfoPage, SessionInfoPage, SortDirection } from '../models/device.models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl: string;
  
  constructor(private http: HttpClient) { 
    this.apiUrl = this.getApiUrl();
    console.log('ApiService initialized with URL:', this.apiUrl);
  }

  private getApiUrl(): string {
    const hostname = window.location.hostname;
    console.log('Current hostname:', hostname);
    
    if (hostname !== 'localhost' && hostname !== '127.0.0.1') {
      return '/api/v1';
    }
    return 'http://localhost:5000/api/v1';
  }
  
  getDevices(offset: number, limit: number, sortDirection: SortDirection): Observable<DeviceInfoPage> {
    const params = new HttpParams()
      .set('limit', limit.toString())
      .set('offset', offset.toString())
      .set('sortDirection', sortDirection.toString());
    
    const url = `${this.apiUrl}/devices`;
    console.log('Making GET request to:', url, 'with params:', params.toString());
    
    return this.http.get<DeviceInfoPage>(url, { params });
  }
  
  getSessions(deviceId: string, offset: number, limit: number, sortDirection: SortDirection): Observable<SessionInfoPage> {
    const params = new HttpParams()
      .set('offset', offset.toString())
      .set('limit', limit.toString())
      .set('sortDirection', sortDirection.toString());

    const url = `${this.apiUrl}/sessions/device/${deviceId}`;
    console.log('Making GET request to:', url, 'with params:', params.toString());
    
    return this.http.get<SessionInfoPage>(url, { params });
  }
  
  deleteSessions(deleteParams: DeleteParameters): Observable<void> {
    const url = `${this.apiUrl}/sessions`;
    console.log('Making DELETE request to:', url, 'with body:', deleteParams);
    
    return this.http.delete<void>(url, {
      body: deleteParams
    });
  }
}