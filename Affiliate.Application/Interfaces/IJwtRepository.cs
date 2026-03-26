public interface IJwtRepository
{
    string GenerateToken(string email, string role);
}