// Направления сортировки
export enum SortDirection {
  Value0 = 0,
  Value1 = 1,
}

// Параметры на удаление записей
export interface DeleteParameters {
  cleanupDate?: string | null;
  deviceId?: string | null;
  sessionIds?: string[] | null;
}

// Информация об устройстве
export interface DeviceInfo {
  id?: string;
  createdAt?: string;
  lastSeenAt?: string;
  sessionsCount?: number;
}

// Страница для пагинации устройств
export interface DeviceInfoPage {
  items?: DeviceInfo[] | null;
  totalItems?: number;  // Изменено с totalCount на totalItems
  totalCount?: number;  // Оставляем для обратной совместимости
}

// Информация о Сессии
export interface SessionInfo {
  id: string;
  deviceId: string;
  name: string;
  startTime: string;
  endTime: string;
  version: string;
  isDeleted?: boolean; 
}

// Страница для пагинации сессий
export interface SessionInfoPage {
  items?: SessionInfo[] | null;
  totalItems?: number;  // Изменено с totalCount на totalItems
  totalCount?: number;  // Оставляем для обратной совместимости
}