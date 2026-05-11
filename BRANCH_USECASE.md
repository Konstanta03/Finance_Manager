# feature-expense-comparison

Гілка містить тільки юзкейс порівняння витрат між періодами.

## CI/CD

У проєкт додано workflow: `.github/workflows/dotnet-ci-cd.yml`.
Він виконує restore, build, test, publish artifact. Для деплою в Azure додай secrets:

- `AZURE_WEBAPP_NAME`
- `AZURE_WEBAPP_PUBLISH_PROFILE`

