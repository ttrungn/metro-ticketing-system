﻿using BuildingBlocks.Domain.Constants;
using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Application.Common.Security;
using OrderService.Application.MomoPayment.DTOs;
using OrderService.Domain.Enums;

namespace OrderService.Application.MomoPayment.Commands.ConfirmMomoPayment;

[Authorize(Roles = Roles.Customer)]
public record ConfirmMomoPaymentCommand : IRequest<ServiceResponse<PaymentResultDto>>
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

public class ConfirmMomoPaymentCommandHandler : IRequestHandler<ConfirmMomoPaymentCommand, ServiceResponse<PaymentResultDto>>
{
    private readonly IMomoService _service;
    private readonly IUser _user;
    private readonly ICartService _cartService;

    private readonly ILogger<ConfirmMomoPaymentCommandHandler> _logger;

    private readonly IOrderService _orderService;
    public ConfirmMomoPaymentCommandHandler(
        ICartService cartService,
        IUser user,
        IMomoService service,
        ILogger<ConfirmMomoPaymentCommandHandler> logger,
        IOrderService orderService)
    {
        _cartService = cartService;
        _user = user;
        _service = service;
        _logger = logger;
        _orderService = orderService;
    }   

    public async Task<ServiceResponse<PaymentResultDto>> Handle(ConfirmMomoPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConfirmMomoPaymentCommandHandler: Handle called with request: {@Request}", request);
        var isConfirm = await _service.ConfirmMomoPaymentAsync(request, cancellationToken);

        if (!isConfirm)
        {
            return new ServiceResponse<PaymentResultDto>
            {
                Succeeded = false,
                Message = "Payment signature invalid.",
                Data = null
            };
        }

        var orderStatus = request.resultCode == 0 ? OrderStatus.Paid : OrderStatus.Cancelled;
        var isValidOrderId = Guid.TryParse(request.OrderId!, out var orderId);
        if (isValidOrderId == false)
        {
            _logger.LogError("Invalid OrderId: {OrderId}", request.OrderId);
            return new ServiceResponse<PaymentResultDto>
            {
                Succeeded = false,
                Message = "Invalid OrderId.",
                Data = null
            };
        }
        var isValidOrder = await _orderService.ConfirmOrder(
            (decimal)request.Amount!,
            request.TransId.ToString()!,
            _user,
            orderId,
            orderStatus,
            request.PayType!);
        if (isValidOrder == 0)
        {
            _logger.LogError("Failed to confirm order for OrderId: {OrderId}", request.OrderId);
            return new ServiceResponse<PaymentResultDto>
            {
                Succeeded = false,
                Message = "Failed to confirm order.",
                Data = null
            };
        }
        await _cartService.RemoveAllCartItemsAsync(_user.Id!, cancellationToken);

        _logger.LogInformation("Payment confirmed for OrderId: {OrderId}, Status: {Status}", request.OrderId, orderStatus);
        var response = new ServiceResponse<PaymentResultDto>
        {
            Succeeded = true,
            Message = "Payment confirmed successfully.",
            Data = new PaymentResultDto
            {
                IsConfirm = true,
                TicketCount = isValidOrder,
                Message = $"Bạn đã mua thành công {isValidOrder} vé."
            }
        };
        return response;
    }
}
