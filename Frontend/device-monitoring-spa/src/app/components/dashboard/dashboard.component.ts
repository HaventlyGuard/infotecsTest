import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/device.service';
import { DeviceInfo, SessionInfo, SortDirection, DeleteParameters } from '../../models/device.models';
import { DeviceListComponent } from '../devise-list/device-list.component';
import { SessionListComponent } from '../session-list/session-list.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe, DeviceListComponent, SessionListComponent],
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

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.loadDevices();
  }

  loadDevices() {
    this.api.getDevices(this.deviceFilter.offset, this.deviceFilter.limit, this.deviceFilter.sortDirection)
      .subscribe({
        next: (response) => {
          this.devices = response?.items || [];
          this.totalDevices = response?.totalCount || 0;
        },
        error: (err) => console.error('Ошибка загрузки устройств', err)
      });
  }

  onDeviceSelected(deviceId: string) {
    this.selectedDeviceId = deviceId;
  }

  onDevicesUpdated() {
    this.loadDevices();
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
}