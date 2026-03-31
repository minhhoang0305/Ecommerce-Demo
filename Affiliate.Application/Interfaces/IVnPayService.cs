public interface IVnPayService
{
    string CreatePaymentUrl(VnPayPaymentRequest request);
    VnPayCallbackResult ValidateCallback(IReadOnlyDictionary<string, string> query);
}
