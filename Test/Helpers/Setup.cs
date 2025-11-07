using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;
using Test.Mocks;

namespace Test.Helpers
{
    public class Setup
    {
        public const string PORT = "5001";
        public static TestContext _testContext = default!;
        public static WebApplicationFactory<Startup> _http = default!;
        public static HttpClient _client = default!;
        private static string? _token;

        public static void ClassInit(TestContext testContext)
        {
            _testContext = testContext;
            _http = new WebApplicationFactory<Startup>();

            _http = _http.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("https_port", PORT).UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAdministratorService, AdministratorServiceMock>();
                });

            });

            _client = _http.CreateClient();
        }

        public static void ClassCleanup()
        {
            _http.Dispose();
        }

        public static async Task AuthenticateAsAdmin()
        {
            await Authenticate("admin@test.com", "123456");
        }

        public static async Task AuthenticateAsEditor()
        {
            await Authenticate("editor@test.com", "123456");
        }

        public static async Task Authenticate(string email, string password)
        {
            var loginDTO = new LoginDTO { Email = email, Password = password };

            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/administrators/login", content);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var logged = JsonSerializer.Deserialize<LoggedAdmininistratorModelView>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", logged!.Token);
        }

        public static void ClearAuthentication() =>
            _client.DefaultRequestHeaders.Authorization = null;

    }
}