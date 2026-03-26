using MediatR;
using Affiliate.Domain.Entities;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check email tồn tại
        var exists = await _userRepository.ExistsByEmailAsync(request.Email);
        if (exists)
            throw new Exception("Email already exists");

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);


        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = request.Role
        };

        await _userRepository.AddAsync(user);

        return user.Id;
    }
}