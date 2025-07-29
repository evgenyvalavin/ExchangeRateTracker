# CbrCurrencyFavorites - Микросервисная архитектура на .NET 9

## Описание проекта

Микросервисная архитектура с использованием Clean Architecture, включающая:

- **DatabaseMigration** - Сервис миграций базы данных
- **CurrencyService** - Фоновый сервис для получения курсов валют с ЦБ РФ
- **UserService** - Микросервис пользователей (регистрация, логин, логаут)
- **FinanceService** - Микросервис финансов (получение курсов валют по пользователю)
- **ApiGateway** - API Gateway для объединения всех сервисов
- **Common** - Общая библиотека для всех сервисов

## Технологии

- .NET 9
- PostgreSQL
- Entity Framework Core
- JWT Authentication
- Docker & Docker Compose
- Clean Architecture
- gRPC

## Структура базы данных

### Таблица `currency`
- `id` - первичный ключ
- `name` - название валюты
- `rate` - курс валюты к рублю

### Таблица `user`
- `id` - первичный ключ
- `name` - имя пользователя
- `password` - пароль

## Запуск проекта

### Предварительные требования
- .NET 9 SDK
- Docker и Docker Compose

### Команды для запуска

1. Клонируйте репозиторий
2. Запустите все сервисы:
```bash
docker-compose up --build
```

### Порты сервисов
- API Gateway: http://localhost:8080
- User Service: http://localhost:8081
- Finance Service: http://localhost:8082
- Currency Service: (фоновый сервис, не имеет HTTP портов)
- PostgreSQL: localhost:5432

### Проверка работоспособности

Health check endpoints:
- API Gateway: http://localhost:8080/health
- User Service: http://localhost:8081/health
- Finance Service: http://localhost:8082/health
- Currency Service: (фоновый сервис, проверка через логи Docker)

### Swagger документация
- API Gateway: http://localhost:8080/swagger
- User Service: http://localhost:8081/swagger
- Finance Service: http://localhost:8082/swagger

## API Endpoints

### User Service
- `POST /api/auth/register` - Регистрация пользователя
- `POST /api/auth/login` - Вход пользователя
- `POST /api/auth/logout` - Выход пользователя

### Finance Service
- `GET /api/currencies` - Получение курсов валют (требует авторизации)
- `GET /api/currencies/favorites` - Получить избранные валюты
- `POST /api/currencies/favorites` - Добавить валюту в избранные
- `DELETE /api/currencies/favorites/{id}` - Удалить валюту из избранных

### API Gateway (единая точка входа)
- `POST /api/auth/register` - Регистрация пользователя
- `POST /api/auth/login` - Вход пользователя
- `POST /api/auth/logout` - Выход пользователя
- `GET /api/auth/me` - Информация о текущем пользователе
- `GET /api/currencies` - Получить все валюты
- `GET /api/currencies/favorites` - Получить избранные валюты
- `POST /api/currencies/favorites` - Добавить валюту в избранные
- `DELETE /api/currencies/favorites/{id}` - Удалить валюту из избранных

## Тестирование

Запуск unit-тестов:
```bash
dotnet test
```

## Архитектура

Проект использует Clean Architecture с разделением на слои:
- Domain - Доменные модели и интерфейсы
- Application - Бизнес-логика и use cases
- Infrastructure - Реализация репозиториев и внешних сервисов
- Presentation - API контроллеры

## Авторизация

Используется JWT токены для авторизации между сервисами и клиентами.
