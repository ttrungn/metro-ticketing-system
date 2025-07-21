using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Domain.Enums;

namespace OrderService.Application.MomoPayment.Commands.ConfirmMomoPayment;

public record ConfirmMomoPaymentCommand : IRequest<ServiceResponse<string>>
{
    public string? PartnerCode { get; init; }
    public string? OrderId { get; init; }

    public string? RequestId { get; init; }

    public long? Amount { get; init; }

    public string? OrderInfo { get; init; } 

    public string? OrderType { get; init; } 

    public long? TransId { get; init; }

    public int? resultCode { get; init; }

    public string? Message { get; init; }

    public string? PayType { get; init; }

    public long? ResponseTime { get; init; }

    public string? ExtraData { get; init; } 

    public string? Signature { get; init; } 

}

public class ConfirmMomoPaymentCommandValidator : AbstractValidator<ConfirmMomoPaymentCommand>
{
    public ConfirmMomoPaymentCommandValidator()
    {
    }
}

public class ConfirmMomoPaymentCommandHandler : IRequestHandler<ConfirmMomoPaymentCommand, ServiceResponse<string>>
{
    private readonly IMomoService _service;
    private readonly IUser _user;

    private readonly ILogger<ConfirmMomoPaymentCommandHandler> _logger;

    private readonly IOrderService _orderService;
    public ConfirmMomoPaymentCommandHandler(
        IUser user,
        IMomoService service,
        ILogger<ConfirmMomoPaymentCommandHandler> logger,
        IOrderService orderService)
    {
        _user = user;
        _service = service;
        _logger = logger;
        _orderService = orderService;
    }   

    public async Task<ServiceResponse<string>> Handle(ConfirmMomoPaymentCommand request, CancellationToken cancellationToken)
    {
        var isConfirm =  await _service.ConfirmMomoPaymentAsync(request, cancellationToken);

        if (!isConfirm)
        {
            return new ServiceResponse<string>
            {
                Succeeded = false,
                Message = "Payment signature invalid.",
                Data = null
            };
        }

        var orderStatus = request.resultCode == 0 ? OrderStatus.Paid : OrderStatus.Cancelled;
        var orderId = Guid.Parse(request.OrderId!);
        await _orderService.ConfirmOrder(
            (decimal)request.Amount!,
            request.TransId.ToString()!,
            _user.Id,
            orderId,
            orderStatus,                 
            request.OrderId!);


        var response = new ServiceResponse<string>
        {
            Succeeded = true,
            Message = "Payment confirmed successfully.",
            Data = "ok",
        };
        return response;
    }
}
