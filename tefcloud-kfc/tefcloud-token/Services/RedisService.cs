using StackExchange.Redis;
using tefcloud_token.Services.Interfaces;

namespace tefcloud_token.Services
{

    public class RedisService : Redis
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IConfiguration _configuration;
        private static Lazy<ConnectionMultiplexer> _lazyConnection;
        private static ConnectionMultiplexer _connection => _lazyConnection.Value;

        public RedisService(
            ILogger<TokenService> logger,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _configuration = configuration;
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = _configuration["RedisConnectionString"]!;
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
        }

        public async Task<bool> StoreTokenAsync(string token)
        {
            try
            {
                IDatabase cache = _connection.GetDatabase();
                bool isSetToken = await cache.StringSetAsync("access_token", token);
                if (isSetToken)
                {
                    _logger.LogInformation("Token stored in Redis successfully.");
                    return true;
                }

                _logger.LogWarning("Failed to store token in Redis.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while storing token in Redis: {ex.Message}");
                return false;
            }
        }

        public async Task<string> GetTokenAsync()
        {
            try
            {
                IDatabase cache = _connection.GetDatabase();
                RedisValue token = await cache.StringGetAsync("access_token");
                if (token.IsNullOrEmpty) 
                {
                    _logger.LogInformation("Token retrieved in Redis successfully.");
                    return token.ToString();
                }

                _logger.LogInformation("Token no found in Redis Cache.");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while storing token in Redis: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
