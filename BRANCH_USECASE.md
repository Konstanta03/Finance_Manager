# base

Базова гілка без юзкейсів планових платежів, фільтрації, пошуку і порівняння витрат.

## CI/CD

У проєкт додано workflow: `.github/workflows/dotnet-ci-cd.yml`.
Він виконує restore, build, test, publish artifact. Для деплою в Azure додай secrets:

- `AZURE_WEBAPP_NAME`
- `AZURE_WEBAPP_PUBLISH_PROFILE`

CI/CD rerun