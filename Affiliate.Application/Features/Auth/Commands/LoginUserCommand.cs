using Affiliate.Application.DTOs.Auth;
using MediatR;

public record LoginUserCommand(LoginRequest Request) : IRequest<AuthResponse>;