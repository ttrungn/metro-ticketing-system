using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Domain.Enums;

namespace OrderService.Application.Orders.Commands.UpdateTicketToUsed;

public record UpdateTicketToUsedCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
}

public class UpdateTicketToUseCommandValidator : AbstractValidator<UpdateTicketToUsedCommand>
{
    public UpdateTicketToUseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Vui lòng cung cấp Id vé cần cập nhật!");

        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Vui lòng cung cấp Id vé!");
    }
}

public class
    UpdateTicketToUsedCommandHandler : IRequestHandler<UpdateTicketToUsedCommand,
    ServiceResponse<Guid>>
{
    private readonly IOrderService _orderService;
    private readonly ILogger<UpdateTicketToUsedCommandHandler> _logger;
    private readonly IUser _user;

    public UpdateTicketToUsedCommandHandler(IOrderService orderService,
        ILogger<UpdateTicketToUsedCommandHandler> logger, IUser user)
    {
        _orderService = orderService;
        _logger = logger;
        _user = user;
    }

    public async Task<ServiceResponse<Guid>> Handle(UpdateTicketToUsedCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating ticket to use for user: {UserId}, TicketId: {TicketId}",
            _user.Id, request.TicketId);

        var (id, ticketId) = await _orderService.UpdateTicketAsync(
            _user.Id,
            request.Id,
            request.TicketId,
            PurchaseTicketStatus.Unused,
            PurchaseTicketStatus.Used
            , cancellationToken);

        if (ticketId == Guid.Empty)
        {
            _logger.LogInformation(
                "Ticket is already used for user: {UserId}, TicketId: {TicketId}", _user.Id,
                request.TicketId);
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Vé đã được cập nhật trạng thái sang đang sử dụng trước đó!",
                Data = id
            };
        }

        _logger.LogInformation(
            "Ticket updated successfully for user: {UserId}, TicketId: {TicketId}", _user.Id,
            ticketId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true, Message = "Cập nhật vé thành công!", Data = id
        };
    }
}
