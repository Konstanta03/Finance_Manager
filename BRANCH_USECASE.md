# feature-transaction-search

Гілка містить тільки юзкейс пошуку транзакцій за нотаткою або категорією.

## CI/CD

У проєкт додано workflow: `.github/workflows/dotnet-ci-cd.yml`.
Він виконує restore, build, test, publish artifact. Для деплою в Azure додай secrets:

- `AZURE_WEBAPP_NAME`
- `AZURE_WEBAPP_PUBLISH_PROFILE`

