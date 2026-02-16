import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/device.service';
import { DeviceInfo, SortDirection, DeviceInfoPage } from '../../models/device.models';
import { DeviceListComponent } from '../device-list/device-list.component';
import { SessionListComponent } from '../session-list/session-list.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, DeviceListComponent, SessionListComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.less']
})
export class DashboardComponent implements OnInit {
  devices: DeviceInfo[] = [];
  selectedDeviceId: string | null = null;
  totalDevices = 0;
  
  deviceFilter = {
    limit: 10,
    offset: 0,
    sortDirection: SortDirection.Value1
  };

  errorMessage: string | null = null;

  constructor(
    private api: ApiService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadDevices();
  }

  loadDevices() {
    this.errorMessage = null;
    
    this.api.getDevices(this.deviceFilter.offset, this.deviceFilter.limit, this.deviceFilter.sortDirection)
      .subscribe({
        next: (response: DeviceInfoPage) => {
          if (response) {
            if (response.items && Array.isArray(response.items)) {
              this.devices = [...response.items];
            }
            
            if (response.totalItems !== undefined) {
              this.totalDevices = response.totalItems;
            } else {
              this.totalDevices = this.devices.length;
            }
          }
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Ошибка загрузки устройств', err);
          this.errorMessage = err.message || 'Unknown error';
          this.devices = [];
          this.totalDevices = 0;
          this.cdr.detectChanges();
        }
      });
  }

  onDeviceSelected(deviceId: string) {
    this.selectedDeviceId = deviceId;
    this.cdr.detectChanges();
  }

  onDevicesUpdated() {
    this.loadDevices();
  }

  refreshSessions() {
    if (this.selectedDeviceId) {
      this.onDeviceSelected(this.selectedDeviceId);
    }
  }

  nextPage() {
    this.deviceFilter.offset += this.deviceFilter.limit;
    this.loadDevices();
  }

  prevPage() {
    if (this.deviceFilter.offset > 0) {
      this.deviceFilter.offset -= this.deviceFilter.limit;
      this.loadDevices();
    }
  }

  getCurrentPage(): number {
    return Math.floor(this.deviceFilter.offset / this.deviceFilter.limit) + 1;
  }

  getTotalPages(): number {
    return Math.ceil(this.totalDevices / this.deviceFilter.limit) || 1;
  }
}