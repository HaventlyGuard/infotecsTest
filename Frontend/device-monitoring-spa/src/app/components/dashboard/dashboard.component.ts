import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/device.service';
import { DeviceInfo, SortDirection, DeviceInfoPage } from '../../models/device.models';
import { DeviceListComponent } from '../devise-list/device-list.component';
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

  constructor(private api: ApiService) {
    console.log('DashboardComponent initialized');
  }

  ngOnInit() {
    console.log('DashboardComponent ngOnInit');
    this.loadDevices();
  }

  loadDevices() {
    console.log('Loading devices with filter:', this.deviceFilter);
    
    this.api.getDevices(this.deviceFilter.offset, this.deviceFilter.limit, this.deviceFilter.sortDirection)
      .subscribe({
        next: (response: DeviceInfoPage) => {
          console.log('Devices response:', response);
          
          // Проверяем структуру ответа
          if (response && response.items) {
            this.devices = response.items || [];
            this.totalDevices = response.totalCount || 0;
          } else if (Array.isArray(response)) {
            // Если API возвращает просто массив
            this.devices = response;
            this.totalDevices = response.length;
          }
          
          console.log('Processed devices:', this.devices);
          console.log('Total devices:', this.totalDevices);
        },
        error: (err) => {
          console.error('Ошибка загрузки устройств', err);
          this.devices = [];
          this.totalDevices = 0;
        }
      });
  }

  onDeviceSelected(deviceId: string) {
    console.log('Device selected:', deviceId);
    this.selectedDeviceId = deviceId;
  }

  onDevicesUpdated() {
    console.log('Devices updated, reloading...');
    this.loadDevices();
  }

  nextPage() {
    console.log('Next page, current offset:', this.deviceFilter.offset);
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