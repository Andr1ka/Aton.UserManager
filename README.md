# API Управления Пользователями

Веб-сервис на .NET 9 для управления пользователями с CRUD операциями, аутентификацией и контролем доступа на основе ролей.

## Требования

- .NET 9 SDK
- PostgreSQL
- Git

## Начало работы

1. Клонируйте репозиторий:
```bash
git clone https://github.com/Andr1ka/Aton.UserManager.git
cd Aton.UserManager
```

2. Восстановите зависимости:
```bash
dotnet restore
```

3. Обновите строку подключения к базе данных в `appsettings.json`:
```json
"ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=user_manager;Username=your_username;Password=your_password"
}
```

4. Примените миграции базы данных:
```bash
dotnet ef database update --project Persistence --startup-project WebApi
```

5. Запустите приложение:
```bash
dotnet run --project WebApi
```

API будет доступен по адресу `https://localhost:7036`, а Swagger UI по адресу `https://localhost:7036/swagger`.

## Начальная настройка

### Создание пользователя-администратора

Для тестирования приложения необходимо создать пользователя-администратора. Это можно сделать только напрямую через базу данных:

1. Подключитесь к базе данных PostgreSQL
2. Выполните следующий SQL-запрос:
```sql
INSERT INTO "Users" (
    "Id", 
    "Login", 
    "Password", 
    "Name", 
    "Gender", 
    "Birthday", 
    "Admin", 
    "CreatedOn", 
    "CreatedBy", 
    "ModifiedOn", 
    "ModifiedBy"
) VALUES (
    gen_random_uuid(),
    'admin',
    'admin123',
    'Administrator',
    1,
    '1990-01-01',
    true,
    CURRENT_TIMESTAMP,
    'system',
    CURRENT_TIMESTAMP,
    'system'
);
```

## Примечания

- Все даты в формате UTC
- Логин и пароль могут содержать только латинские буквы и цифры
- Имя может содержать латинские и русские буквы
- Значения пола: 0 (женский), 1 (мужской), 2 (неизвестно)
- Система автоматически отслеживает:
  - Время создания записи (`CreatedOn`)
  - Время последнего изменения (`ModifiedOn`)
  - Кто создал запись (`CreatedBy`)
  - Кто последний раз изменил запись (`ModifiedBy`)


