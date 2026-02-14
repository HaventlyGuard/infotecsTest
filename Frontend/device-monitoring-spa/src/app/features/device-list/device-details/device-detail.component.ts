import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DeviceService } from '../../../services/device.service';
import { DeviceSession } from '../../../models/device.models';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-device-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './device-detail.component.html',
  styleUrl: './device-detail.component.less'
})
export class DeviceDetailComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  deviceService = inject(DeviceService);
  
  today = new Date().toISOString().split('T')[0];
  sessions: DeviceSession[] = [];
  loading = false;
  error: string | null = null;
  deleteBeforeDate: string = new Date().toISOString().split('T')[0];
  deviceId: string | null = null;
  
  private sessionsSubscription: Subscription | null = null;

  ngOnInit(): void {
    this.deviceId = this.route.snapshot.paramMap.get('id');
    if (this.deviceId) {
      this.deviceService.selectedDeviceId.set(this.deviceId);
      this.loadSessions();
      
      this.sessionsSubscription = this.deviceService.sessions$.subscribe({
        next: (data) => {
          this.sessions = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('Ошибка загрузки сессий:', err);
          this.error = 'Не удалось загрузить сессии устройства';
          this.loading = false;
        }
      });
    }
  }

  ngOnDestroy(): void {
    this.sessionsSubscription?.unsubscribe();
  }

  loadSessions(): void {
    if (!this.deviceId) return;
    
    this.loading = true;
    this.error = null;
  }

  deleteOldSessions(): void {
    if (!this.deviceId) return;

    const beforeDateISO = new Date(this.deleteBeforeDate).toISOString();
    
    if (confirm(`Удалить все записи до ${new Date(this.deleteBeforeDate).toLocaleDateString()}?`)) {
      this.deviceService.deleteOldSessions(this.deviceId, beforeDateISO).subscribe({
        next: () => {
          alert('Старые записи успешно удалены');
          this.loadSessions(); 
        },
        error: (err) => {
          console.error('Ошибка удаления:', err);
          alert('Не удалось удалить записи');
        }
      });
    }
  }

  getDuration(startTime: string, endTime: string): string {
    const start = new Date(startTime).getTime();
    const end = new Date(endTime).getTime();
    const durationMs = end - start;
    
    if (durationMs < 0) return 'Некорректная длительность';
    
    const hours = Math.floor(durationMs / (1000 * 60 * 60));
    const minutes = Math.floor((durationMs % (1000 * 60 * 60)) / (1000 * 60));
    
    if (hours > 0) {
      return `${hours} ч ${minutes} мин`;
    }
    return `${minutes} мин`;
}

}