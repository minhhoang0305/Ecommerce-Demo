using Affiliate.Application.DTOs;
using MediatR;

public record GetUserOrdersQuery(Guid UserId) : IRequest<IReadOnlyList<OrderDTO>>;
