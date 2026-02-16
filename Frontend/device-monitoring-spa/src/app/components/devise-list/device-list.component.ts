import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { DeviceInfo } from '../../models/device.models';

@Component({
  selector: 'app-device-list',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.less']
})
export class DeviceListComponent {
  @Input() devices: DeviceInfo[] = [];
  @Input() selectedDeviceId: string | null = null;
  @Output() deviceSelected = new EventEmitter<string>();
  @Output() devicesUpdated = new EventEmitter<void>();

  selectDevice(deviceId: string | undefined) {
    if (deviceId) {
      this.deviceSelected.emit(deviceId);
    }
  }

  formatDeviceId(id: string): string {
  if (!id) return '';
  return id.length > 12 ? id.substring(0, 12) + '...' : id;
    }

isDeviceOnline(device: DeviceInfo): boolean {
  if (!device.lastSeenAt) return false;
  const lastSeen = new Date(device.lastSeenAt).getTime();
  const now = Date.now();
  const fiveMinutes = 5 * 60 * 1000;
  return (now - lastSeen) < fiveMinutes;
    }
}