# ConferenceBookingApi

REST API для управління бронюванням конференц-залів, розрахунку вартості з урахуванням динамічних цінових зон та аналітики.

---

## 🏛 Архітектура проєкту

Проєкт побудований за принципами **багатошарової архітектури (Layered / Clean Architecture)** з чітким розділенням відповідальності та строгим напрямком залежностей:

```text
ConferenceBooking.slnx
│
├── ConferenceBooking.Services.Web         # Слой представлення: API Контролери, DTO, Swagger, Middleware
├── ConferenceBooking.Bll                  # Слой бізнес-логіки: Менеджери (RoomManager, BookingManager, PricingManager, ReportManager)
├── ConferenceBooking.Bll.Common           # Слой доменних моделей, інтерфейсів менеджерів/репозиторіїв та винятків
├── ConferenceBooking.Dal.SqlRepositories  # Слой даних: MSSQL, ADO.NET, Хранить процедури, TVP, Entities
└── ConferenceBooking.Utils.DbUp           # Універсальний утилітний раннер міграцій на базі DbUp
```

### Основні правила архітектури:
* **Ізоляція домену:** `BLL` залежить виключно від `Bll.Common` і не знає про деталі SQL чи HTTP.
* **Без ORM:** Доступ до даних реалізовано через чистий **ADO.NET (`Microsoft.Data.SqlClient`)** та **Хранить процедури (Stored Procedures)**.
* **AutoMapper на кордонах шарів:**
  * `Services.Web/Mapping` — мапінг `DTO ↔ Domain Model`
  * `Dal.SqlRepositories/Mapping` — мапінг `Database Entity ↔ Domain Model`
* **Автоматичні міграції:** При старті застосунку **DbUp** автоматично створює схему, таблиці, застосовує сидові дані та оновлює процедури.

---

## 📋 Endpoints

### 🏢 Зали (`/api/rooms`)

| Метод | Шлях | Опис |
| :--- | :--- | :--- |
| `GET` | `/api/rooms` | Отримати список усіх залів з доступними послугами |
| `GET` | `/api/rooms/{id}` | Отримати деталі конкретного залу за ID |
| `POST` | `/api/rooms` | Створити новий зал та закріпити послуги |
| `PUT` | `/api/rooms/{id}` | Оновити інформацію про зал та його послуги |
| `DELETE` | `/api/rooms/{id}` | Видалити зал (Soft Delete) |
| `GET` | `/api/rooms/available` | Пошук доступних залів за часом (`start`, `end`) та місткістю (`capacity`) |

### 📅 Бронювання (`/api/bookings`)

| Метод | Шлях | Опис |
| :--- | :--- | :--- |
| `POST` | `/api/bookings` | Створити нове бронювання з перевіркою конфліктів та розрахунком ціни |
| `GET` | `/api/bookings/{id}` | Отримати деталі бронювання (з розбиттям вартості по зонах) |
| `GET` | `/api/bookings/room/{roomId}` | Отримати всі бронювання конкретного залу |

### 📊 Звіти та Аналітика (`/api/reports`)

| Метод | Шлях | Опис |
| :--- | :--- | :--- |
| `GET` | `/api/reports/revenue` | Звіт про виручку за період (`from`, `to`) з розбивкою по днях |
| `GET` | `/api/reports/popularity` | Рейтинг залів за кількістю бронювань та виручкою |
| `GET` | `/api/reports/load` | Відсоток завантаженості залів за період |

---

## 💡 Ключові технічні та бізнес-рішення

### 1. Розрахунок вартості за ціновими зонами (`PricingManager`)
Вартість залу розраховується динамічно залежно від часу доби:

| Зона | Часовий інтервал | Множник |
| :--- | :--- | :--- |
| **Ранкова знижка** | 06:00 – 09:00 | ×0.90 |
| **Стандарт (ранок)** | 09:00 – 12:00 | ×1.00 |
| **Пік** | 12:00 – 14:00 | ×1.15 |
| **Стандарт (день)** | 14:00 – 18:00 | ×1.00 |
| **Вечірня знижка** | 18:00 – 23:00 | ×0.80 |

*Вартість обраних додаткових послуг додається одноразово до загального чеку.*

### 2. Захист від овербукінгу на рівні БД
Перевірка перетинів часових інтервалів виконується атомарно в транзакції процедури `sp_Bookings_Insert` з блокуванням діапазону:
```sql
IF EXISTS (
    SELECT 1 FROM IPiskurovSchema.Bookings WITH (UPDLOCK, HOLDLOCK)
    WHERE RoomId = @RoomId AND StartTime < @EndTime AND EndTime > @StartTime
)
BEGIN
    THROW 50001, 'Зал уже забронирован на указанное время.', 1;
END
```
Це на 100% захищає від стану гонки (Race Condition) без необхідності блокувань у пам'яті веб-сервера.

### 3. Централізована обробка помилок
`GlobalExceptionHandlerMiddleware` перехоплює доменні винятки та повертає уніфікований JSON:
* `RoomNotFoundException` ➔ `404 Not Found`
* `BookingConflictException` ➔ `409 Conflict`
* `InvalidBookingTimeException` ➔ `400 Bad Request`

---

## 🚀 Запуск та тестування

### 1. Авторизація в Azure (Microsoft Entra ID)
Для доступу до бази даних виконайте вхід через Azure CLI:
```bash
az login
```

### 2. Запуск API
```bash
dotnet run --project ConferenceBooking.Services.Web/ConferenceBooking.Services.Web.csproj
```

* **Swagger UI:** [http://localhost:5280](http://localhost:5280)
* **REST тести:** Відкрийте файл [`ConferenceBooking.http`](ConferenceBooking.http) у VS Code (розширення *REST Client*) або JetBrains Rider для виконання готових тестових запитів.
