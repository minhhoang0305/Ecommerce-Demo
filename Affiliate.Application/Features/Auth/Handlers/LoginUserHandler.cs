using Affiliate.Application.DTOs.Auth;
using MediatR;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtRepository _jwtRepository;

    public LoginUserHandler(IUserRepository userRepository, IJwtRepository jwtRepository)
    {
        _userRepository = userRepository;
        _jwtRepository = jwtRepository;
    }
    public async Task<AuthResponse> Handle(LoginUserCommand loginUserCommand, CancellationToken cancellationToken)
    {
        var request = loginUserCommand.Request;
        // Check email tồn tại
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            throw new Exception("Invalid email or password");
        // Check password
        var validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!validPassword)
            throw new Exception("Invalid email or password");
        return new AuthResponse
        {
            AccessToken = _jwtRepository.GenerateToken(user.Email, user.Role),
            infoUser = new 
            {
                user.Id,
                user.Email,
                user.Role
            }
        };

    }

}