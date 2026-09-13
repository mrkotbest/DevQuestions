# Теория: инверсия зависимостей и DI-контейнер

Тема, на которой держится вся архитектура курса. Разбирается один раз здесь, дальше
разборы шагов на неё ссылаются.

Сразу предупреждение: четыре похожих термина означают четыре разные вещи, и их постоянно
путают. Разберём по порядку.

---

## 1. Проблема, которую решают

Начнём с кода, где никакой инверсии нет.

```csharp
public class QuestionsService
{
    private readonly QuestionsEfCoreRepository _repository = new QuestionsEfCoreRepository();

    public async Task<Guid> Create(CreateQuestionDto dto, CancellationToken ct)
    {
        // ...
        await _repository.AddAsync(question, ct);
    }
}
```

Работает. В чём беда?

**Сервис намертво сцеплен с EF Core.** Чтобы создать `QuestionsService`, нужен
`QuestionsEfCoreRepository`, ему нужен `DbContext`, тому — строка подключения и живая база.
Следствия:

1. **Тест сценария требует базы.** Проверить правило «не больше трёх открытых вопросов»
   нельзя без поднятого Postgres.
2. **Замена технологии — правка сервиса.** Решили половину запросов делать на Dapper —
   лезем в бизнес-логику.
3. **Application-слой зависит от инфраструктуры.** Проект со сценариями вынужден ссылаться
   на EF Core, а значит стрелка зависимости смотрит наружу — прямо против правила чистой архитектуры.

Третий пункт самый важный: бизнес-правила — самая ценная и долгоживущая часть системы.
Базы, брокеры и облака вокруг них меняются; логика «вопрос закрывается выбором решения» —
нет. Ставить ценное в зависимость от переменчивого — плохая сделка.

---

## 2. Dependency Inversion Principle — сам принцип

Буква D в SOLID. Формулировка Роберта Мартина в двух частях:

> **Модули верхнего уровня не должны зависеть от модулей нижнего уровня. Оба должны зависеть
> от абстракций.**
>
> **Абстракции не должны зависеть от деталей. Детали должны зависеть от абстракций.**

«Верхний уровень» — это про близость к бизнес-задаче, а не про расположение в вызовах.
`QuestionsService` (сценарий «создать вопрос») — верхний уровень. `QuestionsEfCoreRepository`
(как положить строку в таблицу) — нижний.

Естественный порядок вещей: кто вызывает — тот и зависит. Сервис вызывает репозиторий,
значит зависит от него. **Инверсия** — это переворот стрелки:

```
БЫЛО:   QuestionsService ──зависит──> QuestionsEfCoreRepository

СТАЛО:  QuestionsService ──зависит──> IQuestionsRepository
                                              ▲
                                              │ реализует
                                   QuestionsEfCoreRepository
```

Вызов по-прежнему идёт сверху вниз. А вот **зависимость** теперь направлена снизу вверх:
репозиторий знает про интерфейс, сервис про репозиторий — нет.

### Ключевая мысль, которую обычно упускают

Недостаточно просто выделить интерфейс. Важно, **кому он принадлежит**.

Если `IQuestionsRepository` лежит в проекте `Infrastructure.PostgreSql`, то Application
всё равно ссылается на инфраструктуру — интерфейс есть, инверсии нет. Ничего не изменилось.

Поэтому в проекте так:

```
src/DevQuestions.Application/Questions/IQuestionsRepository.cs           ← интерфейс здесь
src/DevQuestions.Infrastructure.PostgreSql/Repositories/
                                    QuestionsEfCoreRepository.cs          ← реализация здесь
```

и в `DevQuestions.Infrastructure.PostgreSql.csproj`:

```xml
<ProjectReference Include="..\DevQuestions.Application\DevQuestions.Application.csproj" />
```

Инфраструктура ссылается на Application. Не наоборот. **Интерфейс принадлежит тому, кто им
пользуется, а не тому, кто его реализует** — вот и вся инверсия.

Отсюда же вытекает, как должен выглядеть такой интерфейс: его форму диктует сценарий,
а не база данных. `GetOpenUserQuestionsCountAsync(userId)` — это язык предметной области.
Если бы интерфейс писала инфраструктура, там появилось бы что-то вроде
`ExecuteScalar(string sql, object parameters)` — и смысл инверсии потерялся бы.

---

## 3. Четыре термина, которые путают

| Термин | Что означает |
|---|---|
| **Dependency Inversion (DIP)** | Принцип проектирования: зависеть от абстракций, и абстракция принадлежит потребителю |
| **Inversion of Control (IoC)** | Более широкая идея: управление потоком отдано наружу. Любой фреймворк — это IoC: не ты вызываешь ASP.NET, а он вызывает твой контроллер |
| **Dependency Injection (DI)** | Приём: зависимости передаются объекту снаружи, обычно в конструктор, вместо создания внутри |
| **DI-контейнер** | Библиотека, которая делает это автоматически. В .NET — `IServiceCollection`/`IServiceProvider` |

Важно: **DI возможен без контейнера**. Это обычный конструктор:

```csharp
var service = new QuestionsService(new QuestionsEfCoreRepository(dbContext), validator, logger);
```

Это полноценная инъекция зависимостей. Контейнер лишь избавляет от ручной сборки, когда
графы становятся большими. И наоборот: контейнер можно использовать так, что никакого DIP
не получится — если регистрировать конкретные классы и тащить их напрямую.

---

## 4. Как это выглядит в коде

```csharp
public class QuestionsService : IQuestionsService
{
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IValidator<CreateQuestionDto> _createQuestionValidator;
    private readonly ILogger<QuestionsService> _logger;

    public QuestionsService(
        IQuestionsRepository questionsRepository,
        IValidator<CreateQuestionDto> createQuestionValidator,
        ILogger<QuestionsService> logger)
    {
        _questionsRepository = questionsRepository;
        _createQuestionValidator = createQuestionValidator;
        _logger = logger;
    }
}
```

Три признака правильного внедрения через конструктор:

- **Поля `readonly`** — после сборки объекта зависимость не подменить, состояние предсказуемо.
- **Все зависимости в конструкторе** — сигнатура честно перечисляет всё, что нужно классу
  для работы. Забыть передать невозможно, компилятор не даст.
- **Никаких `new` внутри** — класс ничего не создаёт сам, только пользуется данным.

Есть ещё внедрение через свойство и через метод. В .NET они почти не используются:
внедрение через свойство допускает существование объекта в недособранном состоянии, и это
источник `NullReferenceException`. Конструктор — выбор по умолчанию.

### Побочный эффект, который полезнее самого DI

Длина списка параметров конструктора — бесплатный индикатор качества. Три-четыре зависимости
нормально. Восемь — почти наверняка класс делает слишком много и просится на разделение.
Без DI этот сигнал не виден: зависимости прячутся внутри методов.

---

## 5. DI-контейнер в .NET

### Регистрация

```csharp
services.AddScoped<IQuestionsRepository, QuestionsEfCoreRepository>();
```

Читается так: «когда кто-нибудь попросит `IQuestionsRepository`, создай
`QuestionsEfCoreRepository` и отдавай один и тот же экземпляр в пределах одного запроса».

Контейнер строит граф сам: увидев, что `QuestionsEfCoreRepository` требует в конструкторе
`QuestionsDbContext`, он создаст и его, и всё, что нужно ему.

### Времена жизни

Три варианта, и выбор между ними — не формальность.

| Время жизни | Сколько живёт | Для чего |
|---|---|---|
| `Singleton` | всё время работы приложения | кэши, конфигурация, клиенты без состояния (HTTP, S3) |
| `Scoped` | один HTTP-запрос | всё, что связано с запросом: `DbContext`, репозитории, сервисы |
| `Transient` | новый на каждое обращение | лёгкие объекты без состояния |

Почему сервисы и репозитории зарегистрированы как `Scoped`: с ними связан `DbContext`,
а он не потокобезопасен и накапливает состояние — отслеживает загруженные сущности.
Один контекст на запрос означает, что все репозитории внутри запроса работают с одной
единицей работы и одной транзакцией. Сделай его `Singleton` — и параллельные запросы
начнут ломать друг другу отслеживание.

### Ловушка захваченной зависимости

Самая частая ошибка с временами жизни:

```csharp
services.AddSingleton<ICacheService, CacheService>();   // живёт вечно
// а внутри CacheService в конструкторе — QuestionsDbContext (Scoped)
```

Singleton создаётся один раз и **навсегда удерживает** переданный ему Scoped-объект.
Контекст, который должен был умереть вместе с первым запросом, живёт всё время работы
приложения: копит отслеживаемые сущности, держит соединение и перестаёт видеть чужие изменения.

Правило простое: **зависимость не может жить дольше того, кто её захватил**.
Singleton может зависеть только от Singleton. Именно это проверяет `ValidateScopes`,
и именно поэтому такие проверки включены в Development.

### Проверка на старте

При запуске в Development ASP.NET Core включает `ValidateOnBuild` и `ValidateScopes`:
на `builder.Build()` контейнер обходит все регистрации и пробует построить граф каждой.
Если кому-то не хватает зависимости — падение происходит **сразу при старте**, с понятным
сообщением:

```
Unable to resolve service for type 'ISearchProvider'
while attempting to activate 'QuestionsService'
```

Читать такое сообщение нужно с конца: последний упомянутый тип — тот, кого контейнер
не нашёл. В Production эти проверки по умолчанию выключены ради скорости старта; включить
принудительно можно так:

```csharp
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = true;
    options.ValidateScopes = true;
});
```

Рекомендую включать — лучше не стартовать вовсе, чем упасть на первом запросе пользователя.

---

## 6. Composition Root

Место, где собирается весь граф зависимостей, называется **корнем композиции**. Правило:
он должен быть **ровно один и максимально близко к точке входа**.

В проекте он организован в два уровня:

```csharp
// каждый слой знает, что регистрировать ему
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddValidatorsFromAssembly(typeof(DependecyInjection).Assembly);
    services.AddScoped<IQuestionsService, QuestionsService>();
    return services;
}

// хост только собирает слои вместе
public static IServiceCollection AddProgramDependencies(this IServiceCollection services)
{
    services.AddWeb();
    services.AddApplication();
    services.AddPostgreSqlInfrastructure();
    return services;
}
```

Что это даёт: добавляя сервис, правишь файл **того слоя**, которому он принадлежит.
Хост не знает состава слоёв — только их имена. При переходе к модульному монолиту (видео 2.10)
эта конструкция превратится в `AddQuestionsModule()`, `AddTagsModule()` — то есть приём
масштабируется без переписывания.

Возврат `IServiceCollection` из каждого метода — тоже не случайность: он позволяет писать
цепочкой, как это делает сам автор:

```csharp
services.AddWeb().AddApplication().AddPostgreSqlInfrastructure();
```

---

## 7. Service Locator — чего делать не стоит

Приём, который выглядит похоже, но является антипаттерном:

```csharp
public class QuestionsService
{
    private readonly IServiceProvider _provider;   // сам контейнер как зависимость

    public async Task<Guid> Create(...)
    {
        var repository = _provider.GetRequiredService<IQuestionsRepository>();   // достаём по требованию
    }
}
```

Чем плохо:

- **Зависимости спрятаны.** По конструктору больше не видно, что нужно классу. Узнаешь в рантайме.
- **Тест ломается неочевидно.** Нужно поднимать контейнер вместо передачи заглушки.
- **Класс зависит от инфраструктуры DI.** Application-слой начинает знать про `IServiceProvider` —
  ровно то, от чего уходили.

Исключение одно: сам корень композиции и точки, где фреймворк создаёт объекты за тебя
(например, получение scope в фоновой задаче). Внутри бизнес-кода — не стоит.

---

## 8. Что это даёт в тестах

Главная практическая выгода, ради которой всё делается:

```csharp
[Fact]
public async Task Create_Throws_When_User_Has_Too_Many_Open_Questions()
{
    var repository = Substitute.For<IQuestionsRepository>();
    repository.GetOpenUserQuestionsCountAsync(userId, Arg.Any<CancellationToken>())
              .Returns(3);

    var service = new QuestionsService(repository, validator, NullLogger<QuestionsService>.Instance);

    await Assert.ThrowsAsync<InvalidOperationException>(
        () => service.Create(dto, CancellationToken.None));
}
```

Ни базы, ни Docker, ни строки подключения. Тест выполняется за миллисекунды и проверяет
ровно бизнес-правило. Именно поэтому наличие второй реализации у интерфейса — не гипотетика:
**заглушка в тесте и есть вторая реализация**, и она появляется всегда.

---

## 9. Когда инверсия не нужна

Честная граница, потому что принцип легко довести до абсурда.

Интерфейс оправдан, если выполнено хотя бы одно:

- нужна подмена в тесте (почти всегда для всего, что ходит во внешний мир);
- реально существует или планируется вторая реализация;
- нужно провести границу между слоями или модулями.

Интерфейс лишний, если:

- у него одна реализация, она никогда не подменяется, и в тесте её подменять бессмысленно
  (класс с чистыми вычислениями, объект-значение, DTO);
- он механически повторяет публичные методы класса, добавляя только файл — так называемый
  «интерфейс-близнец».

Показательный пример из самого проекта: `ILogger<T>` — интерфейс из коробки, и это правильно
(реализаций много: консоль, Serilog, заглушка в тесте). А вот интерфейс над `CreateQuestionValidator`
не нужен: у FluentValidation уже есть `IValidator<T>`, и именно он внедряется.

---

## 10. Как это собрано в DevQuestions

```
DevQuestions.Application                      объявляет, что нужно:
  Questions/IQuestionsRepository.cs             доступ к вопросам
  Database/ISqlConnectionFactory.cs             соединение с БД
  FullTextSearch/ISearchProvider.cs             полнотекстовый поиск
  FilesStorage/IFileProvider.cs                 файловое хранилище
        ▲                    ▲                    ▲
        │                    │                    │  ссылаются на Application
  Infrastructure.      Infrastructure.      Infrastructure.
    PostgreSql           ElasticSearch          S3
  (EF Core, Dapper)     (Elasticsearch)      (AWS S3)
```

Четыре интерфейса в Application — это полный список того, что приложение требует от внешнего
мира. Он помещается на один экран и читается как техническое задание для инфраструктуры.

А `Web` — единственное место, где эти два мира встречаются: он ссылается на всех и в корне
композиции говорит, какой адаптер отвечает за какой порт.

---

## Проверь себя

1. Чем Dependency Inversion отличается от Dependency Injection? Может ли быть одно без другого?
2. Почему `IQuestionsRepository` обязан лежать в Application? Что изменится, если перенести
   его в Infrastructure, оставив всё остальное как есть?
3. Почему `DbContext` регистрируют как `Scoped`, а не `Singleton`?
4. Что такое захваченная зависимость и как её ловит контейнер?
5. Почему внедрение через конструктор лучше, чем через свойство?
6. Назови интерфейс в проекте, который оправдан, и придумай такой, который был бы лишним.
