# Як запустити тести

## Visual Studio
1. Відкрий `FinanceManager2.0.sln`.
2. Дочекайся NuGet Restore.
3. Відкрий `Test > Test Explorer`.
4. Натисни `Run All Tests`.

У Solution має бути два проєкти:
- `FinanceManager2.0` — основний веб-додаток;
- `FinanceManager.Tests` — тестовий проєкт.

## PowerShell / Terminal
```powershell
dotnet restore FinanceManager2.0.sln
dotnet test FinanceManager2.0.sln
```

Тести використовують `InMemoryDatabase`, а не реальну PostgreSQL/Azure базу.
