using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

public class VnPayService : IVnPayService
{
    private readonly VnPayOptions _options;

    public VnPayService(IOptions<VnPayOptions> options)
    {
        _options = options.Value;
    }

    public string CreatePaymentUrl(VnPayPaymentRequest request)
    {
        EnsureConfigured();

        var now = DateTime.UtcNow.AddHours(7);
        var orderReference = request.OrderId.ToString("N");
        var parameters = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = _options.TmnCode,
            ["vnp_Amount"] = Convert.ToInt64(Math.Round(request.Amount * 100, 0, MidpointRounding.AwayFromZero), CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),
            ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            ["vnp_CurrCode"] = "VND",
            ["vnp_IpAddr"] = request.IpAddress,
            ["vnp_Locale"] = "vn",
            ["vnp_OrderInfo"] = request.OrderInfo,
            ["vnp_OrderType"] = "other",
            ["vnp_ReturnUrl"] = request.ReturnUrl,
            ["vnp_TxnRef"] = orderReference,
            ["vnp_ExpireDate"] = now.AddMinutes(15).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)
        };

        var hashData = BuildQuery(parameters);
        var secureHash = ComputeHmacSha512(_options.HashSecret, hashData);
        var queryString = BuildQuery(parameters.Append(new KeyValuePair<string, string>("vnp_SecureHash", secureHash)));

        return $"{_options.BaseUrl}?{queryString}";
    }

    public VnPayCallbackResult ValidateCallback(IReadOnlyDictionary<string, string> query)
    {
        EnsureConfigured();

        query.TryGetValue("vnp_SecureHash", out var secureHash);
        if (string.IsNullOrWhiteSpace(secureHash))
        {
            return new VnPayCallbackResult(false, false, null, 0, "", "", "Missing secure hash.");
        }

        var parameters = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var item in query)
        {
            if (string.Equals(item.Key, "vnp_SecureHash", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(item.Key, "vnp_SecureHashType", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var value = item.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                parameters[item.Key] = value;
            }
        }

        var hashData = BuildQuery(parameters);
        var calculatedHash = ComputeHmacSha512(_options.HashSecret, hashData);
        var isValidSignature = secureHash.Equals(calculatedHash, StringComparison.OrdinalIgnoreCase);

        var amountRaw = parameters.TryGetValue("vnp_Amount", out var amountValue)
            ? amountValue
            : "0";
        var amount = decimal.TryParse(amountRaw, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsedAmount)
            ? parsedAmount / 100m
            : 0m;

        var responseCode = parameters.TryGetValue("vnp_ResponseCode", out var rc) ? rc : "";
        var transactionStatus = parameters.TryGetValue("vnp_TransactionStatus", out var ts) ? ts : "";
        var orderReference = parameters.TryGetValue("vnp_TxnRef", out var txnRef) ? txnRef : null;
        var isSuccess = isValidSignature && responseCode == "00" && transactionStatus == "00";

        return new VnPayCallbackResult(
            isValidSignature,
            isSuccess,
            orderReference,
            amount,
            responseCode,
            transactionStatus,
            isSuccess ? "Payment confirmed." : "Payment was not successful.");
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_options.TmnCode) || string.IsNullOrWhiteSpace(_options.HashSecret))
        {
            throw new InvalidOperationException("VNPay is not configured. Please set VnPay:TmnCode and VnPay:HashSecret.");
        }
    }

    private static string BuildQuery(IEnumerable<KeyValuePair<string, string>> parameters)
    {
        return string.Join("&", parameters.Select(pair => $"{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(pair.Value)}"));
    }

    private static string ComputeHmacSha512(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
