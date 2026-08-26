using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using MercuryOMS.Application.IServices;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Domain.Entities;
using System.Net;

namespace MercuryOMS.Infrastructure.Services;

public class VnPayPaymentService : IPaymentStrategyService
{
    private readonly IConfiguration _configuration;

    public string Method => "VNPAY";

    public VnPayPaymentService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<PaymentResult> CreatePaymentAsync(
        Guid orderId,
        decimal amount,
        CancellationToken cancellationToken)
    {
        var payment = new Payment(orderId, amount, Method);

        string baseUrl = _configuration["VnPay:BaseUrl"]!;
        string tmnCode = _configuration["VnPay:TmnCode"]!;
        string hashSecret = _configuration["VnPay:HashSecret"]!;
        string returnUrl = _configuration["VnPay:ReturnUrl"]!;

        var vnTime = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

        var expire = vnTime.AddMinutes(15);

        var requestData = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = tmnCode,
            ["vnp_Amount"] = ((long)(amount * 100)).ToString(),
            ["vnp_CreateDate"] = vnTime.ToString("yyyyMMddHHmmss"),
            ["vnp_ExpireDate"] = expire.ToString("yyyyMMddHHmmss"),
            ["vnp_CurrCode"] = "VND",
            ["vnp_IpAddr"] = "127.0.0.1",
            ["vnp_Locale"] = "vn",
            ["vnp_OrderInfo"] = $"Thanh toan don hang {orderId}",
            ["vnp_OrderType"] = "other",
            ["vnp_ReturnUrl"] = returnUrl,
            ["vnp_TxnRef"] = orderId.ToString("N")
        };

        var query = BuildQuery(requestData);

        var secureHash = HmacSHA512(hashSecret, query);

        var paymentUrl =
            $"{baseUrl}?{query}&vnp_SecureHashType=HMACSHA512&vnp_SecureHash={secureHash}";

        return Task.FromResult(new PaymentResult
        {
            Payment = payment,
            PaymentUrl = paymentUrl
        });
    }

    private static string BuildQuery(SortedDictionary<string, string> data)
    {
        var sb = new StringBuilder();

        foreach (var item in data)
        {
            if (string.IsNullOrEmpty(item.Value))
                continue;

            sb.Append(WebUtility.UrlEncode(item.Key));
            sb.Append('=');
            sb.Append(WebUtility.UrlEncode(item.Value));
            sb.Append('&');
        }

        if (sb.Length > 0)
            sb.Length--;

        return sb.ToString();
    }

    private static string HmacSHA512(string key, string input)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(input);

        using var hmac = new HMACSHA512(keyBytes);

        return Convert.ToHexString(hmac.ComputeHash(inputBytes)).ToLowerInvariant();
    }
}