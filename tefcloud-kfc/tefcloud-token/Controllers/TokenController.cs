using Microsoft.AspNetCore.Mvc;
using tefcloud_token.Services;
using tefcloud_token.Services.Interfaces;

namespace tefcloud_token.Controllers
{
    [ApiController]
    [Route("api/tokens")]
    public class TokenController : Controller
    {
        private readonly Token _token;
        private readonly ILogger<TokenService> _logger;
        private readonly Redis _redisCache;

        public TokenController(
            Token token,
            Redis redisCache,
            ILogger<TokenService> logger
        )
        {
            _token = token;
            _redisCache = redisCache;
            _logger = logger;
        }

        [HttpPost("token")]
        public async Task<IActionResult> token()
        {
            _logger.LogInformation($"Function executed at: {DateTime.Now}");
            string token = await _token.GetTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation($"Token retrieved successfully: {token}");
                bool isStored = await _redisCache.StoreTokenAsync(token);
                return Ok(new { access_token = isStored, dateGenerate = DateTime.Now });
            }
            else
            {
                _logger.LogError("Failed to retrieve token.");
                return BadRequest();
            }
        }

        [HttpGet("token")]
        public async Task<IActionResult> getToken()
        {
            string token = await _token.GetTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                return Ok(new { access_token = token });
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
