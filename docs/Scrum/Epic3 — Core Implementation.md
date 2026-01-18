# Epic 3 — Core Implementation

**Цель:** реализовать ядро Uncertainty.Core строго по зафиксированному контракту (Epic 2): тип, операторы, фабрики, базовая статистика, корректное поведение на границах доменов.  
**Гейт после эпика:** готовая, тестируемая, работающая реализация UDouble (или выбранного имени) + полный набор юнит-тестов.

## Выходные артефакты
- реализованный тип (структура/класс) согласно core-contract.md  
- операторы и математические функции  
- фабрики FromDoubleWithRounding / FromFloatWithRounding / FromMeanVariance / FromMeanStdDev  
- корректное поведение на краях доменов  
- статистические функции  
- полный набор тестов  
- документация по реализации

---

## 3.1. Story — Реализация структуры типа (поля, свойства, Immutable)

**Смысл:** создать минимальный рабочий тип согласно core-contract.md: неизменяемый, компактный, переносимый.

### Скоуп
- readonly struct или sealed class (решено в Epic 2)
- поля Mean, Variance
- вычисляемое свойство StdDev
- внутренние конструкторы + публичные фабрики
- неизменяемость данных

### Выходные артефакты
- файл UDouble.cs

### Acceptance Criteria
- тип immutable  
- Variance ≥ 0  
- корректное поведение для NaN/Inf описано и реализовано  

### DoD
- тесты на immutability  
- тесты на корректность Mean/Variance/StdDev  

### Tasks
- создать тип и базовые поля  
- добавить StdDev и инварианты  
- написать тесты  

---

## 3.2. Story — Фабрики и политика округления

**Смысл:** реализовать создание UDouble по спецификации Epic 2.

### Скоуп
- фабрики:
  - FromMeanVariance
  - FromMeanStdDev
  - FromDoubleWithRounding
  - FromFloatWithRounding
- политика преобразования простых double/float
- запрет или разрешение implicit (согласно контракту)

### Выходные артефакты
- файл UDoubleFactory.cs или статические фабрики в типе

### Acceptance Criteria
- корректная Variance для всех фабрик
- чёткое поведение NaN/Infinity
- тесты на каждую фабрику

### DoD
- фабрики соответствуют core-contract.md  
- тесты покрывают все случаи  

### Tasks
Tasks (исправленные под контракт)
T3.2.1 — Implement FromMeanVar
Реализовать базовую фабрику без скрытой логики.
Проверки: variance >= 0, finite.
T3.2.2 — Implement FromMeanStd
stdDev² → variance.
Проверки: stdDev ≥ 0, finite.
T3.2.3 — Implement FromDouble (with IEEE-754 rounding model)
вычислить ulp(x) через экспоненту double
variance = (0.5 * ulp(x))²
запрет NaN / ∞
T3.2.4 — Implement FromFloat (аналогично double)
ulp для float
variance = (0.5 * ulp(x))²
T3.2.5 — Implement explicit conversions
explicit operator UDouble(double x) → FromDouble(x)
explicit operator UDouble(float x) → FromFloat(x)
explicit operator UDouble(int x) → преобразовать x → double → FromDouble(x)
T3.2.6 — Implement FromData(IEnumerable<T>)
Шаги:
привести каждый элемент к UDouble
mean = average(Means)
sigma²_stat = Σ (Meanᵢ – mean)² / (n – 1)
variance_stat = sigma²_stat / n
variance_inst = average(Varianceᵢ)
variance_total = variance_stat + variance_inst
return FromMeanVar(mean, variance_total)
T3.2.7 — Full XML documentation
Документация для всех фабрик, включая explicit conversions.
T3.2.8 — Write tests
На каждый кейс:
корректные значения
ulp-соответствие
NaN/∞ → исключения
FromData:
одинаковые точки
разные точки
разные дисперсии
смешанные типы (int, float, double, UDouble) 

---

## 3.3. Story — Арифметические операторы (+, –, *, /)

**Смысл:** реализовать арифметику по линейной модели распространения неопределённости.

### Скоуп
- операторы +, –, *, /
- корректная Variance:
  - Sum: Va + Vb
  - Mul: b² Va + a² Vb
  - Div: Va / b² + a² Vb / b⁴
- edge cases:
  - деление на 0
  - переполнение
  - NaN/Infinity

### Выходные артефакты
- операторы в UDouble.cs  
- тесты на арифметику и edge-cases  

### Acceptance Criteria
- результаты Mean и Variance корректны  
- поведение согласовано с core-signatures.md  

### DoD
- тесты покрывают все операторы  

### Tasks
- реализовать операторы  
- написать тесты  
- проверить нет ли конфликтов с контрактом  


Task 3.3.1 — Выравнивание с контрактами
Проверить требования к арифметике в:
core-contract.md
core-signatures.md
conversions-and-comparisons.md

Зафиксировать:
формулы Variance;
запреты (implicit, NaN/Inf, отрицательная Variance);
поведение division by zero и saturation.
Артефакт: внутренний checklist соответствия (без кода).

Task 3.3.2 — Реализация операторов + и –
Реализовать operator +, operator - для UDouble.
Mean: арифметика double.
Variance: Va + Vb.
Без saturation, без специальных веток.
Артефакт: код операторов в UDouble.cs.

Task 3.3.3 — Реализация оператора * (умножение)
Реализовать operator *.
Variance: b² Va + a² Vb.
Проверка переполнений → допускается Infinity.
Артефакт: код оператора *.

Task 3.3.4 — Реализация оператора / (деление)
Реализовать operator /.
Проверка b.Mean == 0 → исключение.
Малые b.Mean → saturation Variance согласно контракту.
Формула Variance: Va / b² + a² Vb / b⁴.
Артефакт: код оператора /.

Task 3.3.5 — Инварианты и защитные проверки
Гарантировать:
Variance ≥ 0;
отсутствие NaN/Inf в Variance на выходе (кроме saturation).
Использовать единый internal helper при необходимости.
Артефакт: локальные проверки в коде.

Task 3.3.6 — Юнит-тесты: базовая арифметика
Тесты на:
Exact ∘ Exact → Variance = 0;
коммутативность +, *;
антисимметрию -;
согласованность Mean.
Артефакт: ArithmeticBasicTests.

Task 3.3.7 — Юнит-тесты: формульная проверка Variance
Ручные эталонные кейсы с заранее посчитанной дисперсией.
Отдельные тесты для +, *, /.
Артефакт: ArithmeticVarianceTests.

Task 3.3.8 — Юнит-тесты: edge cases
Division by zero → исключение.
Большие значения → Infinity без падения.
Проверка saturation при делении.
Артефакт: ArithmeticEdgeCaseTests.

Task 3.3.9 — Contract verification
Проверить, что реализация не нарушает:
immutable-модель;
explicit-policy;
правила сравнения и преобразований.
Артефакт: отметка Story как соответствующей контракту.

---

## 3.4. Story — Элементарные математические функции

**Смысл:** реализовать минимальный набор функций с корректной пропагацией неопределённости через производные.

### Скоуп
- Sqrt(x)
- Ln(x)
- Exp(x)
- Sin(x), Cos(x), Tan(x)
- Pow(x, n)
- edge-cases:
  - Sqrt(x<0)
  - Ln(x≤0)
  - тангенс ±π/2

### Выходные артефакты
- файл UDoubleMath.cs  
- тесты значений и вариаций  

### Acceptance Criteria
- ошибки Variance рассчитываются как (f'(x))² Var(x)  
- вне домена выбрасываются корректные исключения  

### DoD
- тесты для всех функций  

### Tasks
- реализовать функции  
- добавить тесты  
- проверить домены  

---

## 3.5. Story — Сравнения и проверки (==, !=, <, >)

**Смысл:** реализовать сравнения по спецификации Epic 2.

### Скоуп
- строгие сравнения < и >
- равенство:
  - через диапазон Mean ± StdDev  
  - или через ε (согласно контракту)
- поведение NaN/Infinity

### Выходные артефакты
- операторы в UDouble.cs  
- тесты  

### Acceptance Criteria
- == реализовано строго по модели  
- <, > используют только Mean  
- представлены edge-cases  

### DoD
- тесты покрывают весь набор сравнений  

### Tasks
- реализовать сравнения  
- написать тесты  
- описать edge-cases  

---

## 3.6. Story — Базовая статистика

**Смысл:** добавить статистические функции для работы с коллекциями неопределённых чисел.

### Скоуп
- Mean(IEnumerable<UDouble>)
- Variance
- SampleVariance
- WeightedMean
- корректная агрегация Variance

### Выходные артефакты
- файл UDoubleStats.cs  
- тесты  

### Acceptance Criteria
- статистика корректна  
- тесты покрывают различные масштабы значений  

### DoD
- все функции статистики имеют тесты  

### Tasks
- реализовать Mean/Variance  
- реализовать SampleVariance  
- написать тесты  

---

## 3.7. Story — Документация реализации

**Смысл:** минимальная документация, синхронизированная с Epic 2.

### Скоуп
- описание реализованных операторов
- таблица edge-cases
- примеры использования

### Выходные артефакты
- файл docs/core-implementation.md  

### Acceptance Criteria
- документ согласован с core-signatures.md  
- есть примеры, которые компилируются  

### DoD
- документ оформлен и проверен  

### Tasks
- описать операторы  
- добавить примеры  
- синхронизировать документ  

---

## 3.8. Gate — Core Implementation Ready

**Условия:**  
- реализованный тип полностью соответствует контракту  
- функции работают согласно математической модели  
- тесты проходят  
- документация обновлена  
- готовность к расширению (Geometry / Kalman / Rotations)

