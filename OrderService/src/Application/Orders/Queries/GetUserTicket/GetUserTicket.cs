using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Application.Orders.DTOs;
using OrderService.Domain.Enums;

namespace OrderService.Application.Orders.Queries.GetUserTicket;

public record GetUserTicketQuery(PurchaseTicketStatus Status) : IRequest<ServiceResponse<IEnumerable<TicketDto>>>;

public class GetUserTicketQueryValidator : AbstractValidator<GetUserTicketQuery>
{
    public GetUserTicketQueryValidator()
    {
        RuleFor(x => x.Status)
            .NotNull()
            .IsInEnum()
            .WithMessage("Xin vui lòng chọn trạng thái vé hợp lệ!");
    }
}

public class GetUserTicketQueryHandler : IRequestHandler<GetUserTicketQuery, ServiceResponse<IEnumerable<TicketDto>>>
{
    private readonly IOrderService _orderService;
    private readonly IUser _user;
    private readonly ILogger<GetUserTicketQueryHandler> _logger;

    public GetUserTicketQueryHandler(IOrderService orderService, IUser user, ILogger<GetUserTicketQueryHandler> logger)
    {
        _orderService = orderService;
        _user = user;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<TicketDto>>> Handle(GetUserTicketQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _orderService.GetUserTicketsAsync(_user.Id, request.Status, cancellationToken);
        _logger.LogInformation("Retrieved user tickets successfully for user: {UserId} with status: {Status}", _user.Id, request.Status);

        return new ServiceResponse<IEnumerable<TicketDto>>()
        {
            Succeeded = true, Message = "Lấy danh sách vé thành công!", Data = tickets
        };
    }
}
