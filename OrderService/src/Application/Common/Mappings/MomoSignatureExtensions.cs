using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Common;
using OrderService.Application.MomoPayment.Commands.ConfirmMomoPayment;
using OrderService.Application.OptionModel;

namespace OrderService.Application.Common.Mappings;
public static class MomoSignatureExtensions
{
    public static string GenerateSignature(this ConfirmMomoPaymentCommand command, MomoOptionModel options)
    {
        var rawHash = "accessKey=" + options.AccessKey +
                      "&amount=" + command.Amount +
                      "&extraData=" + command.ExtraData +
                      "&message=" + command.Message +
                      "&orderId=" + command.OrderId +
                      "&orderInfo=" + command.OrderInfo +
                      "&orderType=" + command.OrderType +
                      "&partnerCode=" + command.PartnerCode +
                      "&payType=" + command.PayType +
                      "&requestId=" + command.RequestId +
                      "&responseTime=" + command.ResponseTime +
                      "&resultCode=" + command.resultCode +
                      "&transId=" + command.TransId;

        return HashHelper.HmacSHA256(rawHash, options.SecretKey ?? "");
    }

    public static bool IsValidSignature(this ConfirmMomoPaymentCommand command, MomoOptionModel options)
    {
        var expected = command.GenerateSignature(options);
        return string.Equals(expected, command.Signature, StringComparison.OrdinalIgnoreCase);
    }
}

