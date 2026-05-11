# Деплоймент і тестування через Azure

## Що підготовлено в проєкті

- ASP.NET Core MVC застосунок.
- Авторизація та реєстрація через ASP.NET Core Identity.
- PostgreSQL через Entity Framework Core + Npgsql.
- Health-check endpoint: `/health`.
- Тестовий endpoint: `/test`.
- Connection string береться з `appsettings.json` або з налаштувань Azure App Service.

## Локальний запуск

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Перевірка:

```text
http://localhost:5101/health
http://localhost:5101/test
```

Стартовий адміністратор:

```text
Email: admin@example.com
Password: Admin123!
```

## Деплой в Azure

1. Створити Azure Database for PostgreSQL.
2. Створити базу `money_manager`.
3. У Azure App Service додати connection string:

```text
Name: DefaultConnection
Value: Host=<server>.postgres.database.azure.com;Database=money_manager;Username=<user>;Password=<password>;SSL Mode=Require;Trust Server Certificate=true
Type: PostgreSQL
```

4. Опублікувати проєкт в Azure App Service через Visual Studio або CLI.
5. Виконати міграції для Azure PostgreSQL:

```bash
dotnet ef database update
```

або виконати міграції локально з Azure connection string.

6. Перевірити сайт:

```text
https://<app-name>.azurewebsites.net/health
https://<app-name>.azurewebsites.net/test
```

## Текст для звіту

Деплоймент системи передбачає розгортання ASP.NET Core MVC застосунку на платформі Microsoft Azure. Дані зберігаються в PostgreSQL, а підключення до бази даних налаштовується через конфігурацію Azure App Service. Для перевірки працездатності додано endpoints `/health` та `/test`, які дозволяють швидко перевірити, що сайт запущений і доступний після публікації.
