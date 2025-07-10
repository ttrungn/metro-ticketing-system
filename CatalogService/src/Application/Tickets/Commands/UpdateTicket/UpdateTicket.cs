using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Domain.Enum;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Tickets.Commands.UpdateTicket;

public record UpdateTicketCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public decimal? Price { get; init; } = 0;
    public int? ActiveInDay { get; init; }
    public int? ExpirationInDay { get; init; }
    public TicketTypeEnum? TicketType { get; init; }
}

public class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketCommandValidator()
    {
        RuleFor(t => t.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID vé!");

        RuleFor(t => t.Name)
            .MinimumLength(0).WithMessage("Tên vé không được quá ngắn!");

        RuleFor(t => t.Price)
            .Cascade(CascadeMode.Stop)
            .Must((cmd, price) =>
            {
                if (cmd.TicketType == TicketTypeEnum.SingleUseType)
                    return price == 0;
                return price > 0;
            })
            .WithMessage(cmd =>
            {
                if (cmd.TicketType == TicketTypeEnum.SingleUseType)
                    return "Vé dùng 1 lần phải có giá bằng 0!";
                return "Xin vui lòng nhập giá vé lớn hơn 0!";
            });

        RuleFor(t => t.ActiveInDay)
            .GreaterThan(0).WithMessage("Số ngày kích hoạt vé phải lớn hơn 0!");

        RuleFor(t => t.ExpirationInDay)
            .GreaterThan(0).WithMessage("Thời gian hết hạn vé phải lớn hơn 0!");
    }
}

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, ServiceResponse<Guid>>
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<UpdateTicketCommandHandler> _logger;

    public UpdateTicketCommandHandler(ITicketService ticketService, ILogger<UpdateTicketCommandHandler> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticketId = await _ticketService.UpdateTicket(request, cancellationToken);
        if (ticketId == Guid.Empty)
        {
            _logger.LogWarning("Ticket with ID {ticketId} not found.", request.Id);
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Không tìm thấy vé!",
                Data = Guid.Empty
            };
        }

        _logger.LogInformation("Ticket updated with ID: {ticketId}", ticketId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Chỉnh sửa vé thành công!",
            Data = ticketId
        };
    }
}
