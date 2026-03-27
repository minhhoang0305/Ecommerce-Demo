public interface IJwtRepository
{
    string GenerateToken(Guid userId, string email, string role);
}
