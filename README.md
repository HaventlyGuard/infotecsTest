#Device Monitoring SPA
##Быстрый запуск
Из корневой папки (где находится docker-compose.yaml): docker-compose up --build -d

##После запуска:
Фронтенд: http://localhost:8087
API Swagger: http://localhost:5000/swagger
Adminer (БД): http://localhost:8082


##Технологии фронтенд

Angular 17 (standalone компоненты)
TypeScript
RxJS
LESS
Nginx



##Технологии бекенд

ASP.NET Core 9.0
Entity Framework Core
PostgreSQL 15
Swagger


##Реализация

Фронтенд построен на Angular 17 с использованием standalone компонентов. Основные компоненты: Dashboard (главная панель), DeviceList (список устройств) и SessionList (список сессий). Данные получаются через ApiService с пагинацией. Для обновления UI используется ChangeDetectorRef. Реализовано удаление сессий (выборочно и по дате). Стили написаны на LESS.
Бекенд на ASP.NET Core 9.0 использует трёхслойную архитектуру: контроллеры обрабатывают HTTP запросы, сервисы содержат бизнес-логику, репозитории работают с БД через Entity Framework. PostgreSQL используется в качестве СУБД. Миграции применяются автоматически при запуске.
Инфраструктура: Docker Compose объединяет все сервисы в общую сеть. Конфигурация через переменные окружения и .env файл.