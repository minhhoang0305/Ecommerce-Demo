public record VnPayCallbackResult(
    bool IsValidSignature,
    bool IsSuccess,
    string? OrderReference,
    decimal Amount,
    string ResponseCode,
    string TransactionStatus,
    string Message);
