import { Component, Input, OnInit, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/device.service';
import { SessionInfo, SortDirection, DeleteParameters, SessionInfoPage } from '../../models/device.models';
import { CeilPipe } from '../../pipes/ceil.pipe';

@Component({
  selector: 'app-session-list',
  standalone: true,
  imports: [CommonModule, FormsModule, CeilPipe],
  templateUrl: './session-list.component.html',
  styleUrls: ['./session-list.component.less']
})
export class SessionListComponent implements OnInit {
  @Input() deviceId!: string;
  @Output() sessionsUpdated = new EventEmitter<void>();
  @Output() refreshRequested = new EventEmitter<void>();

  sessions: SessionInfo[] = [];
  totalSessions = 0;
  selectedSessionIds = new Set<string>();
  isLoading = false;
  
  filter = {
    limit: 15,
    offset: 0,
    sortDirection: SortDirection.Value1
  };

  cleanupDate: string = '';
  showDeleteModal = false;
  deleteMode: 'date' | 'selected' | null = null;

  constructor(
    private api: ApiService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadSessions();
  }

  ngOnChanges() {
    if (this.deviceId) {
      this.filter.offset = 0;
      this.loadSessions();
    }
  }

  loadSessions() {
    if (!this.deviceId) return;

    this.isLoading = true;
    this.cdr.detectChanges();
    
    this.api.getSessions(this.deviceId, this.filter.offset, this.filter.limit, this.filter.sortDirection)
      .subscribe({
        next: (response: SessionInfoPage) => {
          if (response) {
            if (response.items && Array.isArray(response.items)) {
              this.sessions = [...response.items];
            }
            
            if (response.totalItems !== undefined) {
              this.totalSessions = response.totalItems;
            } else {
              this.totalSessions = this.sessions.length;
            }
          }
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Ошибка загрузки сессий', err);
          this.sessions = [];
          this.totalSessions = 0;
          this.isLoading = false;
          this.cdr.detectChanges();
        }
      });
  }

  refresh() {
    this.loadSessions();
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
        this.cdr.detectChanges();
      },
      error: (err) => {
        alert('Ошибка при удалении: ' + (err.error?.message || err.message));
      }
    });
  }

  cancelDelete() {
    this.showDeleteModal = false;
    this.deleteMode = null;
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

  getCurrentPage(): number {
    return Math.floor(this.filter.offset / this.filter.limit) + 1;
  }

  getTotalPages(): number {
    if (this.totalSessions === 0) return 1;
    return Math.ceil(this.totalSessions / this.filter.limit);
  }

  getTotalPagesDisplay(): string {
    const pages = this.getTotalPages();
    return pages.toString();
  }
}