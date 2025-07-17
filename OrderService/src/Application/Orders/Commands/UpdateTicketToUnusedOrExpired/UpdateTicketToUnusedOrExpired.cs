using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Domain.Enums;

namespace OrderService.Application.Orders.Commands.UpdateTicketToUnusedOrExpired;

public record UpdateTicketToUnusedOrExpiredCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
}

public class UpdateTicketToUnusedOrExpiredCommandValidator : AbstractValidator<UpdateTicketToUnusedOrExpiredCommand>
{
    public UpdateTicketToUnusedOrExpiredCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Vui lòng cung cấp Id vé cần cập nhật!");

        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Vui lòng cung cấp Id vé!");
    }
}

public class UpdateTicketToUnusedOrExpiredCommandHandler : IRequestHandler<UpdateTicketToUnusedOrExpiredCommand, ServiceResponse<Guid>>
{
    private readonly IOrderService _orderService;
    private readonly ILogger<UpdateTicketToUnusedOrExpiredCommandHandler> _logger;
    private readonly IUser _user;

    public UpdateTicketToUnusedOrExpiredCommandHandler(IOrderService orderService, ILogger<UpdateTicketToUnusedOrExpiredCommandHandler> logger, IUser user)
    {
        _orderService = orderService;
        _logger = logger;
        _user = user;
    }

    public async Task<ServiceResponse<Guid>> Handle(UpdateTicketToUnusedOrExpiredCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating ticket to unused or expired for user: {UserId}, TicketId: {TicketId}", _user.Id, request.TicketId);

        var (id, ticketId) = await _orderService.UpdateTicketAsync(
            _user.Id,
            request.Id,
            request.TicketId,
            PurchaseTicketStatus.Used,
            null,
            cancellationToken);

        if (ticketId == Guid.Empty)
        {
            _logger.LogInformation("Ticket is already unused or expired for user: {UserId}, TicketId: {TicketId}", _user.Id, request.TicketId);
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Vé đã được cập nhật sang trạng thái không sử dụng hoặc hết hạn trước đó!",
                Data = id
            };
        }

        _logger.LogInformation("Ticket updated successfully for user: {UserId}, TicketId: {TicketId}", _user.Id, ticketId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true, Message = "Cập nhật vé thành công!", Data = id
        };
    }
}
