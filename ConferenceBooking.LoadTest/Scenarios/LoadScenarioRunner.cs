using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using ConferenceBooking.LoadTest.Metrics;
using ConferenceBooking.LoadTest.Models;

namespace ConferenceBooking.LoadTest.Scenarios;

public class LoadScenarioRunner : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly List<Guid> _roomIds = new();
    private bool _disposed;

    public LoadScenarioRunner(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15),
            MaxConnectionsPerServer = 500
        };
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    /// <summary>
    /// Ініціалізація перед тестом: завантажує список наявних залів для використання в сценаріях
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/rooms");
            if (response.IsSuccessStatusCode)
            {
                using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    if (item.TryGetProperty("id", out var idProp) && idProp.TryGetGuid(out var id))
                    {
                        _roomIds.Add(id);
                    }
                }
            }
        }
        catch
        {
            // Якщо не вдалося завантажити зали, будуть згенеровані тимчасові Guid
        }

        if (_roomIds.Count == 0)
        {
            _roomIds.Add(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
            _roomIds.Add(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));
            _roomIds.Add(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"));
        }
    }

    /// <summary>
    /// Виконує навантажувальний тест із заданою кількістю задач та ступенем паралельності
    /// </summary>
    public async Task<TestRunReport> RunTestAsync(int totalRequests, int concurrency)
    {
        var metrics = new MetricsCollector();
        using var semaphore = new SemaphoreSlim(concurrency, concurrency);

        Console.Write($"  Виконання {totalRequests} запитів ({concurrency} concurrent)... ");
        var completedCount = 0;

        var globalSw = Stopwatch.StartNew();

        var tasks = new Task[totalRequests];
        for (var i = 0; i < totalRequests; i++)
        {
            var requestIndex = i;
            tasks[i] = Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await ExecuteRandomScenarioAsync(requestIndex, metrics);
                }
                finally
                {
                    semaphore.Release();
                    Interlocked.Increment(ref completedCount);
                }
            });
        }

        // Фонове оновлення прогресу з миттєвим перериванням через токен
        using var progressCts = new CancellationTokenSource();
        var progressTask = Task.Run(async () =>
        {
            try
            {
                while (!progressCts.Token.IsCancellationRequested && Volatile.Read(ref completedCount) < totalRequests)
                {
                    await Task.Delay(200, progressCts.Token);
                    var current = Volatile.Read(ref completedCount);
                    var percent = (int)((double)current / totalRequests * 100);
                    Console.Write($"\r  Виконання {totalRequests} запитів ({concurrency} concurrent)... {percent}% ({current}/{totalRequests})");
                }
            }
            catch (OperationCanceledException)
            {
                // Завершення прогресу без очікування
            }
        });

        await Task.WhenAll(tasks);
        progressCts.Cancel();
        await progressTask;
        globalSw.Stop();

        Console.WriteLine($"\r  Виконання {totalRequests} запитів ({concurrency} concurrent)... 100% ({totalRequests}/{totalRequests}) Готово! ({globalSw.Elapsed.TotalSeconds:F2} c)");

        return metrics.BuildReport(concurrency, globalSw.Elapsed);
    }

    private async Task ExecuteRandomScenarioAsync(int index, MetricsCollector metrics)
    {
        // Розподіл сценаріїв:
        // 0..24  (25%) - GET  /api/rooms
        // 25..44 (20%) - GET  /api/rooms/{id}
        // 45..59 (15%) - GET  /api/rooms/available
        // 60..69 (10%) - GET  /api/reports/revenue
        // 70..74 (5%)  - GET  /api/reports/popularity
        // 75..89 (15%) - POST /api/bookings
        // 90..94 (5%)  - POST /api/rooms
        // 95..99 (5%)  - PUT  /api/rooms/{id}
        var scenarioChoice = Random.Shared.Next(100);

        var targetRoomId = _roomIds[Random.Shared.Next(_roomIds.Count)];

        if (scenarioChoice < 25)
        {
            await MeasureRequestAsync("GET", "/api/rooms", () => _httpClient.GetAsync("/api/rooms"), metrics);
        }
        else if (scenarioChoice < 45)
        {
            await MeasureRequestAsync("GET", $"/api/rooms/{{id}}", () => _httpClient.GetAsync($"/api/rooms/{targetRoomId}"), metrics);
        }
        else if (scenarioChoice < 60)
        {
            var futureDays = Random.Shared.Next(1, 60);
            var start = DateTime.UtcNow.Date.AddDays(futureDays).AddHours(10).ToString("o");
            var end = DateTime.UtcNow.Date.AddDays(futureDays).AddHours(14).ToString("o");
            var url = $"/api/rooms/available?start={start}&end={end}&capacity=20";
            await MeasureRequestAsync("GET", "/api/rooms/available", () => _httpClient.GetAsync(url), metrics);
        }
        else if (scenarioChoice < 70)
        {
            var url = "/api/reports/revenue?from=2026-08-01T00:00:00Z&to=2026-08-31T23:59:59Z";
            await MeasureRequestAsync("GET", "/api/reports/revenue", () => _httpClient.GetAsync(url), metrics);
        }
        else if (scenarioChoice < 75)
        {
            await MeasureRequestAsync("GET", "/api/reports/popularity", () => _httpClient.GetAsync("/api/reports/popularity"), metrics);
        }
        else if (scenarioChoice < 90)
        {
            // POST /api/bookings
            var futureDays = Random.Shared.Next(1, 30);
            var hour = Random.Shared.Next(9, 17);
            var startTime = DateTime.UtcNow.Date.AddDays(futureDays).AddHours(hour);
            var endTime = startTime.AddHours(2);

            var bookingPayload = new
            {
                roomId = targetRoomId,
                startTime = startTime,
                endTime = endTime,
                selectedServiceIds = Array.Empty<Guid>()
            };

            await MeasureRequestAsync("POST", "/api/bookings", () => _httpClient.PostAsJsonAsync("/api/bookings", bookingPayload), metrics);
        }
        else if (scenarioChoice < 95)
        {
            // POST /api/rooms (AvailableServices відповідає CreateRoomDto)
            var newRoomPayload = new
            {
                name = $"Навантажувальний Зал #{index}",
                capacity = Random.Shared.Next(10, 200),
                baseHourlyRate = (decimal)Random.Shared.Next(1000, 5000),
                availableServices = Array.Empty<object>()
            };

            await MeasureRequestAsync("POST", "/api/rooms", () => _httpClient.PostAsJsonAsync("/api/rooms", newRoomPayload), metrics);
        }
        else
        {
            // PUT /api/rooms/{id} (AvailableServices відповідає UpdateRoomDto)
            var updatePayload = new
            {
                name = $"Оновлений Зал #{targetRoomId.ToString()[..4]}",
                capacity = Random.Shared.Next(20, 150),
                baseHourlyRate = (decimal)Random.Shared.Next(1200, 4000),
                availableServices = Array.Empty<object>()
            };

            await MeasureRequestAsync("PUT", "/api/rooms/{id}", () => _httpClient.PutAsJsonAsync($"/api/rooms/{targetRoomId}", updatePayload), metrics);
        }
    }

    private static async Task MeasureRequestAsync(string method, string endpoint, Func<Task<HttpResponseMessage>> sendFunc, MetricsCollector metrics)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var response = await sendFunc();
            sw.Stop();

            var statusCode = (int)response.StatusCode;
            var isSuccess = response.IsSuccessStatusCode;

            metrics.Record(new RequestResult(
                Method: method,
                Endpoint: endpoint,
                StatusCode: statusCode,
                DurationMs: sw.Elapsed.TotalMilliseconds,
                IsSuccess: isSuccess));
        }
        catch (Exception ex)
        {
            sw.Stop();
            metrics.Record(new RequestResult(
                Method: method,
                Endpoint: endpoint,
                StatusCode: 0,
                DurationMs: sw.Elapsed.TotalMilliseconds,
                IsSuccess: false,
                ErrorMessage: ex.Message));
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}
