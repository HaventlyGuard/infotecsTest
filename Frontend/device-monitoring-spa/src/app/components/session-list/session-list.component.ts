import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/device.service';
import { SessionInfo, SortDirection, DeleteParameters } from '../../models/device.models';

@Component({
  selector: 'app-session-list',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './session-list.component.html',
  styleUrls: ['./session-list.component.less']
})
export class SessionListComponent implements OnInit {
  @Input() deviceId!: string;
  @Output() sessionsUpdated = new EventEmitter<void>();

  sessions: SessionInfo[] = [];
  totalSessions = 0;
  selectedSessionIds = new Set<string>();
  
  filter = {
    limit: 15,
    offset: 0,
    sortDirection: SortDirection.Value1
  };

  cleanupDate: string = '';
  showDeleteModal = false;
  deleteMode: 'date' | 'selected' | null = null;

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.loadSessions();
  }

  loadSessions() {
    this.api.getSessions(this.deviceId, this.filter.offset, this.filter.limit, this.filter.sortDirection)
      .subscribe({
        next: (response) => {
          this.sessions = response?.items || [];
          this.totalSessions = response?.totalCount || 0;
        },
        error: (err) => console.error('Ошибка загрузки сессий', err)
      });
  }

  nextPage() {
    this.filter.offset += this.filter.limit;
    this.loadSessions();
  }

  prevPage() {
    if (this.filter.offset > 0) {
      this.filter.offset -= this.filter.limit;
      this.loadSessions();
    }
  }

  toggleSession(sessionId: string) {
    if (this.selectedSessionIds.has(sessionId)) {
      this.selectedSessionIds.delete(sessionId);
    } else {
      this.selectedSessionIds.add(sessionId);
    }
  }

  toggleAllSessions(event: any) {
    if (event.target.checked) {
      this.sessions.forEach(s => this.selectedSessionIds.add(s.id));
    } else {
      this.selectedSessionIds.clear();
    }
  }

  isSessionSelected(session: SessionInfo): boolean {
    return this.selectedSessionIds.has(session.id);
  }

  getSelectedCount(): number {
    return this.selectedSessionIds.size;
  }

  openDeleteModal(mode: 'date' | 'selected') {
    this.deleteMode = mode;
    this.showDeleteModal = true;
  }

  confirmDelete() {
    const params: DeleteParameters = {
      deviceId: this.deviceId
    };

    if (this.deleteMode === 'selected') {
      params.sessionIds = Array.from(this.selectedSessionIds);
    } else if (this.deleteMode === 'date' && this.cleanupDate) {
      params.cleanupDate = new Date(this.cleanupDate).toISOString();
    }

    if (!params.sessionIds?.length && !params.cleanupDate) {
      alert('Не выбраны элементы для удаления');
      return;
    }

    this.api.deleteSessions(params).subscribe({
      next: () => {
        this.showDeleteModal = false;
        this.deleteMode = null;
        this.selectedSessionIds.clear();
        this.cleanupDate = '';
        this.loadSessions();
        this.sessionsUpdated.emit();
        this.showNotification('Сессии успешно удалены', 'success');
      },
      error: (err) => {
        this.showNotification('Ошибка при удалении: ' + (err.error?.message || err.message), 'error');
      }
    });
  }

  cancelDelete() {
    this.showDeleteModal = false;
    this.deleteMode = null;
  }

  showNotification(message: string, type: 'success' | 'error') {
    alert(message);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString('ru-RU', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}