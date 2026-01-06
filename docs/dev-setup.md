# Development Setup

## Repository Structure
    /src        — исходный код библиотек
    /tests      — проекты с тестами
    /examples   — примеры использования
    /docs       — внутренняя документация

## Requirements
    .NET SDK 8.0+
    Git

## Build
    dotnet build

## Run Tests
    dotnet test

## Run Example
    dotnet run --project examples/Uncertainty.Core.Examples

## CI
GitHub Actions автоматически выполняет сборку и тесты на каждом push и pull request в ветку main.
