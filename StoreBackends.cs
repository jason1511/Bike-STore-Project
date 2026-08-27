using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bike_STore_Project
{
    public sealed record LoginResult(bool Success, int UserId, string Username, string Role, string Error = "", string Token = "");
    public sealed record StoreBrand(string Id, string Name, bool IsActive = true);

    public interface IStoreBackend : IDisposable
    {
        StoreProfile Profile { get; }
        bool SupportsFullDesktopWorkflow { get; }
        bool UsesFifoPurchaseCost { get; }
        Task<LoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
        Task TestConnectionAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<StoreBrand>> GetBrandsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<WebsiteBike>> GetBikesAsync(string? search = null, CancellationToken cancellationToken = default);
        Task<WebsiteBike?> GetBikeAsync(string id, CancellationToken cancellationToken = default);
        Task SaveBikeAsync(WebsiteBike bike, bool isNew, CancellationToken cancellationToken = default);
        Task ReceiveStockAsync(string bikeId, string colorName, string colorHex, string colorImage,
            int quantity, decimal unitCost, DateTime receivedAt, string note, CancellationToken cancellationToken = default);
        Task SetBikeActiveAsync(string bikeId, bool active, CancellationToken cancellationToken = default);
        void SignOut();
    }

    public static class AppServices
    {
        public static StoreProfile Profile { get; private set; } = StoreProfile.CreateDemo();
        public static IStoreBackend Backend { get; private set; } = null!;

        public static void Configure(StoreProfile profile)
        {
            Backend?.Dispose();
            Profile = profile;
            Backend = profile.IsOnline
                ? new CloudflareStoreBackend(profile)
                : new SqliteStoreBackend(profile);
        }
    }

    public sealed class SqliteStoreBackend : IStoreBackend
    {
        private readonly UserRepository _users = new();
        private readonly WebsiteBikeRepository _bikes = new();
        public StoreProfile Profile { get; }
        public bool SupportsFullDesktopWorkflow => true;
        public bool UsesFifoPurchaseCost => true;

        public SqliteStoreBackend(StoreProfile profile) => Profile = profile;

        public Task<LoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var ok = _users.TryLogin(username, password, out var id, out var role, out var error);
            return Task.FromResult(new LoginResult(ok, id, username.Trim().ToLowerInvariant(), role, error));
        }

        public Task TestConnectionAsync(CancellationToken cancellationToken = default)
        {
            using var connection = Database.OpenConnection();
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<StoreBrand>> GetBrandsAsync(CancellationToken cancellationToken = default)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT CAST(id AS TEXT),name,is_active FROM brands WHERE is_active=1 ORDER BY sort_order,name;";
            using var reader = command.ExecuteReader();
            var brands = new List<StoreBrand>();
            while (reader.Read()) brands.Add(new StoreBrand(reader.GetString(0), reader.GetString(1), reader.GetInt32(2) == 1));
            return Task.FromResult<IReadOnlyList<StoreBrand>>(brands);
        }

        public Task<IReadOnlyList<WebsiteBike>> GetBikesAsync(string? search = null, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<WebsiteBike>>(_bikes.GetAll(search));
        public Task<WebsiteBike?> GetBikeAsync(string id, CancellationToken cancellationToken = default)
            => Task.FromResult(_bikes.GetById(id));
        public Task SaveBikeAsync(WebsiteBike bike, bool isNew, CancellationToken cancellationToken = default)
        { _bikes.SaveBike(bike, isNew); return Task.CompletedTask; }
        public Task ReceiveStockAsync(string bikeId, string colorName, string colorHex, string colorImage, int quantity,
            decimal unitCost, DateTime receivedAt, string note, CancellationToken cancellationToken = default)
        { _bikes.ReceiveStock(bikeId, colorName, colorHex, colorImage, quantity, unitCost, receivedAt, note); return Task.CompletedTask; }
        public Task SetBikeActiveAsync(string bikeId, bool active, CancellationToken cancellationToken = default)
        { _bikes.SetActive(bikeId, active); return Task.CompletedTask; }
        public void SignOut() { }
        public void Dispose() { }
    }

    public sealed class CloudflareStoreBackend : IStoreBackend
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        private readonly HttpClient _http;
        private string _token = "";
        public StoreProfile Profile { get; }
        public bool SupportsFullDesktopWorkflow => false;
        public bool UsesFifoPurchaseCost => false;

        public CloudflareStoreBackend(StoreProfile profile)
        {
            Profile = profile;
            var baseUrl = profile.ApiBaseUrl.Trim().TrimEnd('/');
            if (!Uri.TryCreate(baseUrl + "/", UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
                throw new InvalidOperationException("Online store URL must be a valid HTTPS address.");
            _http = new HttpClient { BaseAddress = uri, Timeout = TimeSpan.FromSeconds(30) };
        }

        public async Task<LoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            using var response = await _http.PostAsJsonAsync("api/admin/login", new { username, password }, JsonOptions, cancellationToken);
            var text = await response.Content.ReadAsStringAsync(cancellationToken);
            LoginEnvelope envelope;
            try { envelope = JsonSerializer.Deserialize<LoginEnvelope>(text, JsonOptions) ?? new LoginEnvelope(); }
            catch { envelope = new LoginEnvelope(); }
            if (!response.IsSuccessStatusCode)
                return new LoginResult(false, 0, username, "USER", envelope.Error ?? "Login failed.");
            _token = envelope.Token ?? "";
            if (string.IsNullOrWhiteSpace(_token))
                return new LoginResult(false, 0, username, "USER", "The server did not return a session token.");
            return new LoginResult(true, StableUserId(envelope.Username ?? username), envelope.Username ?? username,
                NormalizeRole(envelope.Role), Token: _token);
        }

        public async Task TestConnectionAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _http.GetAsync("api/bikes", cancellationToken);
            if (!response.IsSuccessStatusCode) throw new InvalidOperationException($"Server returned HTTP {(int)response.StatusCode}.");
        }

        public async Task<IReadOnlyList<StoreBrand>> GetBrandsAsync(CancellationToken cancellationToken = default)
        {
            var envelope = await GetAsync<BrandsEnvelope>("api/admin/brands", cancellationToken);
            return envelope.Brands?.Select(x => new StoreBrand(x.Id ?? "", x.Name ?? "", x.IsActive)).ToList() ?? new();
        }

        public async Task<IReadOnlyList<WebsiteBike>> GetBikesAsync(string? search = null, CancellationToken cancellationToken = default)
        {
            var envelope = await GetAsync<BikesEnvelope>("api/admin/bikes", cancellationToken);
            IEnumerable<WebsiteBike> bikes = envelope.Bikes ?? new();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim();
                bikes = bikes.Where(x => x.Brand.Contains(q, StringComparison.OrdinalIgnoreCase)
                    || x.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
                    || x.ColorSummary.Contains(q, StringComparison.OrdinalIgnoreCase));
            }
            return bikes.ToList();
        }

        public async Task<WebsiteBike?> GetBikeAsync(string id, CancellationToken cancellationToken = default)
            => (await GetBikesAsync(null, cancellationToken)).FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

        public async Task SaveBikeAsync(WebsiteBike bike, bool isNew, CancellationToken cancellationToken = default)
        {
            using var request = Authorized(isNew ? HttpMethod.Post : HttpMethod.Put, "api/admin/bikes");
            request.Content = JsonContent.Create(bike, options: JsonOptions);
            using var response = await _http.SendAsync(request, cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);
        }

        public async Task ReceiveStockAsync(string bikeId, string colorName, string colorHex, string colorImage,
            int quantity, decimal unitCost, DateTime receivedAt, string note, CancellationToken cancellationToken = default)
        {
            var bike = await GetBikeAsync(bikeId, cancellationToken) ?? throw new InvalidOperationException("Bike not found.");
            var color = bike.Colors.FirstOrDefault(x => x.Name.Equals(colorName, StringComparison.OrdinalIgnoreCase));
            if (color == null)
            {
                color = new WebsiteBikeColor { Name = colorName, Hex = colorHex, Image = colorImage, StockQty = 0 };
                bike.Colors.Add(color);
            }
            color.StockQty += quantity;
            if (!string.IsNullOrWhiteSpace(colorHex)) color.Hex = colorHex;
            if (!string.IsNullOrWhiteSpace(colorImage)) color.Image = colorImage;
            if (string.IsNullOrWhiteSpace(bike.ColorName)) bike.ColorName = bike.Colors.FirstOrDefault()?.Name ?? "";
            await SaveBikeAsync(bike, false, cancellationToken);
        }

        public async Task SetBikeActiveAsync(string bikeId, bool active, CancellationToken cancellationToken = default)
        {
            var bike = await GetBikeAsync(bikeId, cancellationToken) ?? throw new InvalidOperationException("Bike not found.");
            bike.InStock = active;
            await SaveBikeAsync(bike, false, cancellationToken);
        }

        private HttpRequestMessage Authorized(HttpMethod method, string path)
        {
            if (string.IsNullOrWhiteSpace(_token)) throw new InvalidOperationException("Sign in to the online store first.");
            var request = new HttpRequestMessage(method, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
            return request;
        }

        private async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken)
        {
            using var request = Authorized(HttpMethod.Get, path);
            using var response = await _http.SendAsync(request, cancellationToken);
            return await ReadAsync<T>(response, cancellationToken);
        }

        private static async Task<T> ReadAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var text = await response.Content.ReadAsStringAsync(cancellationToken);
            T? result = null;
            try { result = JsonSerializer.Deserialize<T>(text, JsonOptions); } catch { }
            if (!response.IsSuccessStatusCode)
            {
                string error = "";
                try { error = JsonSerializer.Deserialize<ErrorEnvelope>(text, JsonOptions)?.Error ?? ""; } catch { }
                throw new HttpRequestException(string.IsNullOrWhiteSpace(error) ? $"Server returned HTTP {(int)response.StatusCode}." : error);
            }
            return result ?? throw new InvalidDataException("The server returned an empty or invalid response.");
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode) return;
            var text = await response.Content.ReadAsStringAsync(cancellationToken);
            string error = "";
            try { error = JsonSerializer.Deserialize<ErrorEnvelope>(text, JsonOptions)?.Error ?? ""; } catch { }
            throw new HttpRequestException(string.IsNullOrWhiteSpace(error) ? $"Server returned HTTP {(int)response.StatusCode}." : error);
        }

        private static int StableUserId(string username)
        {
            unchecked
            {
                var hash = 17;
                foreach (var c in username) hash = hash * 31 + c;
                return Math.Abs(hash == int.MinValue ? 1 : hash) + 1;
            }
        }

        private static string NormalizeRole(string? role) => role?.Equals("admin", StringComparison.OrdinalIgnoreCase) == true ? "ADMIN" : "USER";
        public void SignOut() => _token = "";
        public void Dispose() => _http.Dispose();

        private sealed class LoginEnvelope { public bool Success { get; set; } public string? Token { get; set; } public string? Role { get; set; } public string? Username { get; set; } public string? Error { get; set; } }
        private sealed class BikesEnvelope { public List<WebsiteBike>? Bikes { get; set; } }
        private sealed class BrandsEnvelope { public List<BrandEnvelope>? Brands { get; set; } }
        private sealed class BrandEnvelope { public string? Id { get; set; } public string? Name { get; set; } public bool IsActive { get; set; } = true; }
        private sealed class ErrorEnvelope { public string? Error { get; set; } }
    }
}
