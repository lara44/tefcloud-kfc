namespace tefcloud_token.Services.Interfaces
{
    public interface Redis
    {
        Task<bool> StoreTokenAsync(string token);
        Task<string> GetTokenAsync();
    }
}
