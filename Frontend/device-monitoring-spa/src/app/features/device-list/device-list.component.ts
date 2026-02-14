import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DeviceService } from '../../../app/services/device.service';
import { DeviceInfo } from '../../../app/models/device.models';

@Component({
  selector: 'app-device-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './device-list.component.html',
  styleUrl: './device-list.component.less'
})
export class DeviceListComponent implements OnInit {
  deviceService = inject(DeviceService);
  devices: DeviceInfo[] = [];
  loading = false;
  error: string | null = null;

  ngOnInit(): void {
    this.loadDevices();
  }

  loadDevices(): void {
    this.loading = true;
    this.error = null;
    
    this.deviceService.getDevices().subscribe({
      next: (data) => {
        this.devices = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Ошибка загрузки устройств:', err);
        this.error = 'Не удалось загрузить список устройств';
        this.loading = false;
      }
    });
  }

  selectDevice(deviceId: string): void {
    this.deviceService.selectedDeviceId.set(deviceId);
  }
}