using Newtonsoft.Json.Linq;
using tefcloud_token.Services.Interfaces;

namespace tefcloud_token.Services
{
    public class TokenService : Token
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IConfiguration _configuration;
        private static readonly HttpClient httpClient = new HttpClient();

        public TokenService(
            ILogger<TokenService> logger,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> GetTokenAsync()
        {
            _logger.LogInformation($"GetTokenAsync function executed at: {DateTime.Now}");

            string url = _configuration["TokenUrl"]!;
            _logger.LogInformation($"TokenUrl: {url}");
            var requestBody = new[]
            {
                new KeyValuePair<string, string>("username", _configuration["UsernameTefCloud"]!),
                new KeyValuePair<string, string>("password", _configuration["Password"]!),
                new KeyValuePair<string, string>("client_id", _configuration["ClientId"]!),
                new KeyValuePair<string, string>("client_secret", _configuration["ClientSecret"]!),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var content = new FormUrlEncodedContent(requestBody);

            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseBody);

                if (responseJson != null && responseJson["access_token"] != null)
                {
                    _logger.LogInformation($"Access Token: {responseJson["access_token"]}");
                    return responseJson["access_token"]!.ToString();
                }

                _logger.LogError("Token not found in the response.");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while retrieving token: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
