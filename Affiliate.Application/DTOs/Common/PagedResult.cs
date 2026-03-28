namespace Affiliate.Application.DTOs.Common;

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int Size,
    int TotalCount);
