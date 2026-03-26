using MediatR;
public record GetByIdAsync(Guid Id) : IRequest<Products?>;