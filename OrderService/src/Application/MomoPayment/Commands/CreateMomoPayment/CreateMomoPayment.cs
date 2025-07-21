using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Application.MomoPayment.DTOs;

namespace OrderService.Application.MomoPayment.Commands.CreateMomoPayment;

public record CreateMomoPaymentCommand : IRequest<ServiceResponse<MomoCreatePaymentResponseModel>>
{

    public double? Amount { get; init; }

    public List<OrderDetailDto>? OrderDetails { get; init; }
}

public class CreateMomoPaymentCommandValidator : AbstractValidator<CreateMomoPaymentCommand>
{
    public CreateMomoPaymentCommandValidator()
    {
    }
}

public class CreateMomoPaymentCommandHandler : IRequestHandler<CreateMomoPaymentCommand, ServiceResponse<MomoCreatePaymentResponseModel>>
{

    private readonly ILogger<CreateMomoPaymentCommandHandler> _logger;
    private readonly IMomoService _service;
    private readonly IUser _user;
    private readonly IOrderService _orderService;
    public CreateMomoPaymentCommandHandler(
          ILogger<CreateMomoPaymentCommandHandler> logger,
          IMomoService service,
            IUser user,
            IOrderService orderService
          )
    {
        _logger = logger;
        _service = service;
        _user = user;
        _orderService = orderService;
    }


    public async Task<ServiceResponse<MomoCreatePaymentResponseModel>> Handle(CreateMomoPaymentCommand request, CancellationToken cancellationToken)
    {
        var response = new ServiceResponse<MomoCreatePaymentResponseModel>();

        try
        {
            if (request.Amount == null || request.Amount <= 0)
            {
                response.Succeeded = false;
                response.Message = "Invalid amount.";
                return response;
            }

            if (request.OrderDetails == null || !request.OrderDetails.Any())
            {
                response.Succeeded = false;
                response.Message = "Order details must not be empty.";
                return response;
            }

            var momoResponse = await _service.CreatePaymentWithMomo(request,cancellationToken);

            if (momoResponse == null)
            {
                _logger.LogError("MoMo payment failed: {Message}", momoResponse?.Message ?? "No response");
                response.Succeeded = false;
                response.Message = $"MoMo payment failed: {momoResponse?.Message ?? "Unknown error"}";
                return response;
            }

            _logger.LogInformation("MoMo payment initiated. OrderId: {OrderId}", momoResponse.OrderId);

            var orderId = await _orderService.CreateOrderAsync(momoResponse.OrderId,_user.Id, request.OrderDetails, cancellationToken);    
            
            if (orderId == Guid.Empty)
            {
                _logger.LogError("Failed to create order for MoMo payment. OrderId: {OrderId}", momoResponse.OrderId);
                response.Succeeded = false;
                response.Message = "Failed to create order for MoMo payment.";
                return response;
            }

            response.Succeeded = true;
            response.Message = "MoMo payment created successfully.";
            response.Data = momoResponse;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while processing MoMo payment");
            response.Succeeded = false;
            response.Message = "Internal server error occurred while processing MoMo payment.";
            return response;
        }
    }

}
