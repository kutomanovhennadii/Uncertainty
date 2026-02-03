# 3a. Epic 3a — Реорганизация политик

**Цель:** превратить UncertaintyPolicies в единую публичную точку доступа к настройкам политик, вынести вспомогательные реализации в internal-слой и унифицировать структуру папки src/Uncertainty.Core/Policies/.
**Гейт после эпика:** конфигурация политик доступна только через UncertaintyPolicies, все вспомогательные реализации находятся в Policies/, тесты и документация синхронизированы.

## 3a.1. Story — Анализ текущего состояния

**Смысл:** зафиксировать, какие политики и стратегии существуют сегодня, где они живут и как к ним обращается код.

**Скоуп:**
- проинвентаризировать публичную поверхность src/Uncertainty.Core/Policies/UncertaintyPolicies.cs (свойства, методы, XML-комментарии);
- разобрать обязанности src/Uncertainty.Core/VarianceSaturationPolicy.cs, src/Uncertainty.Core/Policies/DivisionStrategies.cs, src/Uncertainty.Core/Policies/IDivisionStrategy.cs на предмет public/internal;
- собрать текущее покрытие в tests/Uncertainty.Core.Tests/UncertaintyPoliciesTests.cs, tests/Uncertainty.Core.Tests/DivisionBehaviorTests.cs, tests/Uncertainty.Core.Tests/UDoubleArithmeticTests.cs и в документах docs/policies.md, docs/Contracts/core-contract.md.

**Выходные артефакты:** таблица или конспект зависимостей (приложение к epic), список явных точек входа для политик.

**Acceptance Criteria:**
- перечислены все публичные и внутренние точки входа;
- описаны зависимости между фасадом и вспомогательными реализациями;
- задокументированы тесты и документы, которые нужно обновить.

**DoD:** есть артефакт анализа, согласованный в команде, и его ссылка занесена в docs/Scrum/Scrum.md.

**Tasks:**
- T3a.1.1 — Выписать текущие свойства/методы UncertaintyPolicies.
- T3a.1.2 — Описать роли VarianceSaturationPolicy и DivisionStrategies.
- T3a.1.3 — Составить список тестов/доков, затрагивающих политики.

## 3a.2. Story — Проектирование структуры фасада и internal-слоя

**Смысл:** определить, как будет выглядеть единый фасад и каким станет внутреннее устройство папки Policies/.

**Скоуп:**
- предложить схему пространств имён и каталогов (например, Policies/Internal/, Policies/Division/);
- решить, какие API остаются public в UncertaintyPolicies, какие делегируются во internal-хелперы;
- определить формат конфигурационных структур/методов (immutable-настройки, fluent API) и правила обратной совместимости.

**Выходные артефакты:** дизайн-нота в docs/Scrum/Scrum.md, где зафиксированы решения по неймингам и структуре.

**Acceptance Criteria:**
- согласованная структура папки Policies/;
- описаны изменения публичной поверхности и требования к backward compatibility;
- утверждены интерфейсы взаимодействия фасада с internal-классами.

**DoD:** дизайн-нота подписана ключевыми участниками (Core, Docs, QA), ссылки добавлены в Epic3a.

**Tasks:**
- T3a.2.1 — Подготовить схему каталогов и пространств имён.
- T3a.2.2 — Определить список public-членов фасада после реорганизации.


#### Конспект T3a.2.1 — Предлагаемая структура каталогов и пространств имён

```
src/Uncertainty.Core/Policies/
	UncertaintyPolicies.cs            (public фасад, namespace Uncertainty.Core.Policies)
	DivisionPolicies/
		DivisionBehaviorPolicy.cs       (internal registry, namespace Uncertainty.Core.Policies.Division)
		IDivisionStrategy.cs            (public или internal, TBD)
		DivisionStrategies/
			ThrowingDivisionStrategy.cs
			SaturatingDivisionStrategy.cs
			ReturnInfinityDivisionStrategy.cs
	VarianceSaturationPolicies/
		VarianceSaturationPolicy.cs     (internal, namespace Uncertainty.Core.Policies.Variance)
	Shared/
		PolicyValidation.cs             (общие проверки/guard'ы при необходимости)
```

- **Пространства имён:** `Uncertainty.Core.Policies.*` соответствует структуре папок; фасад остаётся в корне `Uncertainty.Core.Policies`.
- **Категории:** подпапки `Division` и `Variance` отражают тип поведения; новые категории (например, `Factories`, `Comparison`) добавляются аналогично.
- **Расширяемость:** каждая категория содержит собственные стратегии/правила, которые экспортируются через фасад, сохраняя единый публичный API.

#### Конспект T3a.2.2 — Публичная поверхность фасада после реорганизации

- **Фасад:** [src/Uncertainty.Core/Policies/UncertaintyPolicies.cs](src/Uncertainty.Core/Policies/UncertaintyPolicies.cs) остаётся единственной точкой входа. В нём группируем публичные члены по двум направлениям — деление и насыщение дисперсии — чтобы исключить прямые обращения к internal-политикам.
- **Блок "Деление":**
	- `double DivisionTolerance { get; }` + `void SetDivisionTolerance(double tolerance)` — текущий порог и валидирующий метод обновления.
	- `DivisionBehavior DivisionBehavior { get; set; }` — выбор встроенной стратегии (`ThrowOnSmallDenominator`, `SaturateVariance`, `ReturnInfinityMean`).
	- `IDivisionStrategy DivisionStrategy { internal get; internal set}` — фактическая стратегия; весь доступ внутренний
	- `enum DivisionBehavior { ThrowOnSmallDenominator, SaturateVariance, ReturnInfinityMean}` 
	- `interface IDivisionStrategy` ([src/Uncertainty.Core/Policies/DivisionPolicies/IDivisionStrategy.cs](src/Uncertainty.Core/Policies/DivisionPolicies/IDivisionStrategy.cs)) делаем приватным. 
- **Блок "Насыщение дисперсии":**
	- `VarianceSaturationOptions VarianceSaturation { get; }` — возвращает текущие ограничения, используемые internal-политикой (максимальное относительное σ и абсолютный потолок дисперсии).
	- `void ConfigureVarianceSaturation(VarianceSaturationOptions options)` — валидирует вход, обновляет параметры и синхронно оповещает internal `VarianceSaturationPolicy`.
	- `readonly record struct VarianceSaturationOptions(double MaxRelativeStdDev, double AbsoluteVarianceMax)` — публичный тип данных с предикатами корректности (значения > 0, MaxRelativeStdDev ≥ 1, AbsoluteVarianceMax ≥ 1e300).
		- `MaxRelativeStdDev` — верхняя граница относительного стандартного отклонения (`StdDev / |Mean|`). Сохраняем текущее значение по умолчанию `1e8`.
		- `AbsoluteVarianceMax` — минимальный абсолютный потолок для дисперсии вне зависимости от масштаба среднего. Дефолт соответствует контракту (`1e300`).
		- `VarianceSaturationOptions.Default` — статическое свойство с текущими значениями VarianceSaturationPolicy; используется при инициализации фасада и для отката к контрактным настройкам.
- **Общие правила совместимости:**
	- Существующие свойства/методы (`DivisionTolerance`, `DivisionBehavior`, `DivisionStrategy`) сохраняют сигнатуры; новые API добавляются без изменений в текущем коде пользователей.
	- Изменение настроек Saturation через новый фасад не нарушает старые значения констант: дефолт `VarianceSaturationOptions` повторяет текущие константы internal-политики.
	- Фасад остаётся потокобезопасным на уровне set-операций за счёт внутренней синхронизации (реализуется в Story 3a.3).

## 3a.3. Story — Реализация реорганизации

**Смысл:** перенести код согласно дизайну, сохранив функциональность и совместимость.

**Скоуп:**
- переместить VarianceSaturationPolicy в src/Uncertainty.Core/Policies/Internal/, сделать internal и настроить доступ через фасад;
- обновить UncertaintyPolicies.cs: делегирование к internal-слою, обработка устаревших членов, поддержка DivisionTolerance/Behavior;
- скорректировать обращение к политикам в src/Uncertainty.Core/UDouble.cs, tests/Uncertainty.Core.Tests/*, examples/Uncertainty.Core.Examples/Demos/DivisionStrategyDemos.cs;
- синхронизировать документацию docs/policies.md, docs/STRUCTURE.md, XML-комментарии.

**Выходные артефакты:** обновлённые файлы в Policies/ и фасад, миграционные заметки.

**Acceptance Criteria:**
- компиляция проходит без ошибок;
- публичная поверхность фасада соответствует утверждённому дизайну;
- нет прямых обращений к internal-политикам извне.

**DoD:** код собран, unit-тесты обновлены, документация отражает изменения.

**Tasks:**
- T3a.3.1 — Переместить/переименовать VarianceSaturationPolicy и обновить ссылки.
- T3a.3.2 — Обновить UncertaintyPolicies с делегированием и шинами совместимости.
- T3a.3.3 — Правки вызовов в UDouble, тестах, примерах.
- T3a.3.4 — Обновить документацию и XML.

## 3a.4. Story — Валидация и регрессии

**Смысл:** убедиться, что поведение не деградировало после реорганизации.

**Скоуп:**
- запустить `dotnet build -c Release`, `dotnet test` для tests/Uncertainty.Core.Tests;
- выполнить целевые сценарии (деление с малыми знаменателями, насыщение, FromData) и примеры (`dotnet run --project examples/Uncertainty.Core.Examples`);
- провести ревью изменений на предмет API-утечек и обновить CHANGELOG/README при необходимости.

**Выходные артефакты:** отчёт о прогоне тестов, отметки в CI, обновлённые заметки в CHANGELOG.

**Acceptance Criteria:**
- тесты и примеры проходят;
- нет утечки internal-типов наружу;
- документация соответствует реализации.

**DoD:** ревью закрыто, CI зелёный, результаты прогонов приложены к epic.

**Tasks:**
- T3a.4.1 — Прогнать build/test локально и в CI.
- T3a.4.2 — Проверить ключевые сценарии вручную/через примеры.
- T3a.4.3 — Обновить CHANGELOG/README при изменении публичной поверхности.

## 3a.5. Story — Коммуникация и документация

**Смысл:** синхронизировать команду по изменённой структуре политик и зафиксировать актуальное состояние в документах проекта.

**Скоуп:**
- обновить docs/policies.md и docs/Contracts/core-contract.md, описав единый фасад и расположение internal-политик;
- отразить прогресс и решения в scrum-логах (docs/Scrum/Scrum.md), обозначить задачи последующих эпиков;
- собрать обратную связь от разработчиков и QA по удобству нового фасада, сформировать список улучшений для следующих итераций.

**Выходные артефакты:** обновлённые документы, заметка в scrum-логе, список улучшений/вопросов.

**Acceptance Criteria:**
- документация соответствует реализованной структуре;
- участники команды понимают, как работать с фасадом и internal-папкой;
- зафиксированы потенциальные улучшения или риски.

**DoD:** ссылки на обновлённые документы опубликованы, обратная связь собрана, решение по следующему шагу задокументировано.

- T3a.5.3 — Описать правила для новых политик и документировать их.

## 3a.Gate — Unified Policies Ready

**Условия:**
- UncertaintyPolicies — единственная публичная точка настройки;
- все вспомогательные политики находятся в Policies/ и скрыты внутри internal-слоя;
- тесты/документация обновлены, команда синхронизирована по изменениям.

### Приложение A — Анализ текущего состояния (Story 3a.1)

**Публичный фасад**
- [src/Uncertainty.Core/Policies/UncertaintyPolicies.cs](src/Uncertainty.Core/Policies/UncertaintyPolicies.cs) — свойства DivisionTolerance, DivisionBehavior, DivisionStrategy; статический конструктор с дефолтами; enum DivisionBehavior.

**Вспомогательные реализации**
- [src/Uncertainty.Core/VarianceSaturationPolicy.cs](src/Uncertainty.Core/VarianceSaturationPolicy.cs) — internal Saturation-политика, определяет константы AbsoluteVarianceMax, MaxRelativeStdDev.
- [src/Uncertainty.Core/Policies/DivisionStrategies.cs](src/Uncertainty.Core/Policies/DivisionStrategies.cs) — internal стратегии Throwing/Saturating/ReturnInfinity, используют VarianceSaturationPolicy.
- [src/Uncertainty.Core/Policies/IDivisionStrategy.cs](src/Uncertainty.Core/Policies/IDivisionStrategy.cs) — публичный интерфейс для стратегий (назначение клиентских реализаций ограничено internal setter'ом фасада).

**Основные точки использования**
- [src/Uncertainty.Core/UDouble.cs](src/Uncertainty.Core/UDouble.cs) — делегирует деление и устаревший DivisionTolerance к фасаду.
- [examples/Uncertainty.Core.Examples/Demos/DivisionStrategyDemos.cs](examples/Uncertainty.Core.Examples/Demos/DivisionStrategyDemos.cs) — демонстрация поведения политик.
- [tests/Uncertainty.Core.Tests/UncertaintyPoliciesTests.cs](tests/Uncertainty.Core.Tests/UncertaintyPoliciesTests.cs), [tests/Uncertainty.Core.Tests/DivisionBehaviorTests.cs](tests/Uncertainty.Core.Tests/DivisionBehaviorTests.cs), [tests/Uncertainty.Core.Tests/UDoubleArithmeticTests.cs](tests/Uncertainty.Core.Tests/UDoubleArithmeticTests.cs) — покрытие поведения и гвардей.

**Документация и соглашения**
- [docs/policies.md](docs/policies.md) — описание дефолтов и сценариев.
- [docs/Contracts/core-contract.md](docs/Contracts/core-contract.md) — ограничения на saturating-политику.

**Выводы для Story 3a.1**
- Требуется выровнять размещение VarianceSaturationPolicy и стратегий внутри папки Policies/.
- Понадобится обновление тестов и документации при изменении путей/видимости.
- IDivisionStrategy публичен, но setter фасада internal — решить, оставляем ли расширяемость или прячем интерфейс в ходе реорганизации.
