public class VnPayOptions
{
    public const string SectionName = "VnPay";

    public string TmnCode { get; set; } = string.Empty;
    public string HashSecret { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
    public string ReturnPath { get; set; } = "/api/v1/payments/vnpay/return";
    public string IpnPath { get; set; } = "/api/v1/payments/vnpay/ipn";
}
