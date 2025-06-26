using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;

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

    private readonly ILogger<ConfirmMomoPaymentCommandHandler> _logger;
   
    public ConfirmMomoPaymentCommandHandler(
        IMomoService service,
        ILogger<ConfirmMomoPaymentCommandHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<ServiceResponse<string>> Handle(ConfirmMomoPaymentCommand request, CancellationToken cancellationToken)
    {
       string msg =  await _service.ConfirmMomoPaymentAsync(request, cancellationToken);
        var response = new ServiceResponse<string>
        {
            Succeeded = true,
            Message = "Payment confirmed successfully.",
            Data = msg
        };
        return response;
    }
}
