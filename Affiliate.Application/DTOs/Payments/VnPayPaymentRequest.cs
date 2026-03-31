public record VnPayPaymentRequest(
    Guid OrderId,
    decimal Amount,
    string OrderInfo,
    string ReturnUrl,
    string IpAddress);
