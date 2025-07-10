using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Domain.Enum;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Tickets.Commands.CreateTicket;

public record CreateTicketCommand : IRequest<ServiceResponse<Guid>>
{
    public string? Name { get; init; }
    public decimal Price { get; init; } = 0;
    public int ActiveInDay { get; init; }
    public int ExpirationInDay { get; init; }
    public TicketTypeEnum TicketType { get; init; }
}

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty().WithMessage("Xin vui lòng nhập tên vé!");

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

        RuleFor(t => t.TicketType)
            .NotNull().WithMessage("Xin vui lòng chọn loại vé!");

    }
}

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, ServiceResponse<Guid>>
{
    private readonly ILogger<CreateTicketCommandHandler> _logger;
    private readonly ITicketService _ticketService;

    public CreateTicketCommandHandler(ILogger<CreateTicketCommandHandler> logger, ITicketService ticketService)
    {
        _logger = logger;
        _ticketService = ticketService;
    }

    public async Task<ServiceResponse<Guid>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticketId = await _ticketService.CreateTicketAsync(request, cancellationToken);

        _logger.LogInformation("Ticket created with ID: {TicketId}", ticketId);

        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Tạo vé thành công!",
            Data = ticketId
        };
    }
}
