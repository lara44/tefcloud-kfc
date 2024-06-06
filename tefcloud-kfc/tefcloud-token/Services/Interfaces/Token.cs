namespace tefcloud_token.Services.Interfaces
{
    public interface Token
    {
        Task<string> GetTokenAsync();
    }
}
