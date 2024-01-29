namespace MoviesApi.Services
{
    public interface IAuthServices
    {
        public string GenerateJwtToken(IdentityUser user);
    }
}
