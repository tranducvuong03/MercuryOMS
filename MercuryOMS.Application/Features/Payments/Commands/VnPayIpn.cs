using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MercuryOMS.Application.Features
{
    public class VnPayIpnCommand : IRequest<Result>
    {
        public Dictionary<string, string> Parameters { get; set; } = [];
    }

    public class VnPayReturnCommandHandler
        : IRequestHandler<VnPayIpnCommand, Result>
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public VnPayReturnCommandHandler(
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            VnPayIpnCommand request,
            CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, string>(request.Parameters);

            if (!parameters.TryGetValue("vnp_SecureHash", out var secureHash))
                return Result.Failure("Thiếu SecureHash.");

            parameters.Remove("vnp_SecureHash");
            parameters.Remove("vnp_SecureHashType");

            var hashSecret = _configuration["VnPay:HashSecret"]!;

            var query = BuildQuery(parameters);

            var calculatedHash = HmacSHA512(hashSecret, query);

            if (!secureHash.Equals(calculatedHash, StringComparison.OrdinalIgnoreCase))
                return Result.Failure("Chữ ký VNPAY không hợp lệ.");

            if (!parameters.TryGetValue("vnp_TmnCode", out var tmnCode))
                return Result.Failure("Thiếu TmnCode.");

            if (tmnCode != _configuration["VnPay:TmnCode"])
                return Result.Failure("TmnCode không hợp lệ.");

            if (!parameters.TryGetValue("vnp_TxnRef", out var txnRef))
                return Result.Failure("Thiếu TxnRef.");

            if (!Guid.TryParseExact(txnRef, "N", out var orderId))
                return Result.Failure("TxnRef không hợp lệ.");

            var payment = await _unitOfWork
                    .GetRepository<Payment>().Query
                    .SingleOrDefaultAsync(
                        p => p.OrderId == orderId,
                        cancellationToken);

            if (payment is null)
                return Result.Failure("Không tìm thấy Payment.");

            // Đã xử lý trước đó thì bỏ qua
            if (payment.Status != PaymentStatus.Pending)
                return Result.Success("Đơn hàng đã được thanh toán thành công");

            if (!parameters.TryGetValue("vnp_Amount", out var amountString))
                return Result.Failure("Thiếu Amount.");

            if (!long.TryParse(amountString, out var amount))
                return Result.Failure("Amount không hợp lệ.");

            if (amount != (long)(payment.Amount * 100))
                return Result.Failure("Số tiền không khớp.");

            var responseCode = parameters.GetValueOrDefault("vnp_ResponseCode");
            var transactionStatus = parameters.GetValueOrDefault("vnp_TransactionStatus");

            if (responseCode == "00" && transactionStatus == "00")
            {
                payment.MarkPaid();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success("Thanh toán thành công");
            }

            payment.MarkFailed($"ResponseCode={responseCode}, TransactionStatus={transactionStatus}");
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Failure($"Thanh toán thất bại với mã lỗi: {responseCode}");
        }

        private static string BuildQuery(Dictionary<string, string> data)
        {
            var sorted = new SortedDictionary<string, string>(
                data,
                StringComparer.Ordinal);

            var sb = new StringBuilder();

            foreach (var item in sorted)
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
            using var hmac = new HMACSHA512(
                Encoding.UTF8.GetBytes(key));

            return Convert.ToHexString(
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(input)))
                .ToLowerInvariant();
        }
    }
}