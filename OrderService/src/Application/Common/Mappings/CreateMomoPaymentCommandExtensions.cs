using System.Text;
using Newtonsoft.Json;
using OrderService.Application.MomoPayment.Commands.CreateMomoPayment;
using OrderService.Application.MomoPayment.DTOs;
using OrderService.Application.OptionModel; // adjust if needed

namespace OrderService.Application.MomoPayment.Extensions;

public static class CreateMomoPaymentCommandExtensions
{
    public static MomoOneTimePaymentModel ToMomoOneTimePayment(
        this CreateMomoPaymentCommand command,
        MomoOptionModel options)
    {
        var orderId = Guid.NewGuid().ToString();
        var requestId = Guid.NewGuid().ToString();

        // Serialize OrderDetails to base64 extraData
        string extraDataJson = JsonConvert.SerializeObject(command.OrderDetails ?? new());
        string extraDataBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(extraDataJson));

        var momoModel = new MomoOneTimePaymentModel
        {
            PartnerCode = options.PartnerCode,
            OrderId = orderId,
            RequestId = requestId,
            Amount = (long)(command.Amount ?? 0),
            OrderInfo = $"Thanh Toán Vé Tàu Metro",
            RedirectUrl = options.RedirectUrl,
            IpnUrl = options.IpnUrl,
            RequestType = options.RequestType,
            ExtraData = extraDataBase64,
            Lang = "vi"
        };

        momoModel.MakeSignature(options.AccessKey ?? "", options.SecretKey ?? "");

        return momoModel;
    }
}
