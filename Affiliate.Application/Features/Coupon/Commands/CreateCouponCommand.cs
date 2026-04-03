using Affiliate.Domain.Entities;
using MediatR;

public record CreateCouponCommand(
    string Code,
    DiscountType DiscountType,
    decimal Value,
    decimal MinOrderValue,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    int UsageLimit) : IRequest<Guid>;
