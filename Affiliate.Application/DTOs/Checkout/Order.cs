namespace Affiliate.Application.DTOs;

public record OrderDTO(Guid Id, List<OrderItemDTO> Items, decimal TotalAmount, bool IsPaid, DateTime CreatedAt);
