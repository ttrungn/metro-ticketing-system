using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Tickets.Commands.DeleteTicket;

public record DeleteTicketCommand(Guid Id) : IRequest<ServiceResponse<Guid>>;

public class DeleteTicketCommandValidator : AbstractValidator<DeleteTicketCommand>
{
    public DeleteTicketCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID của vé!");
    }
}

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, ServiceResponse<Guid>>
{
    private readonly ILogger<DeleteTicketCommandHandler> _logger;
    private readonly ITicketService _ticketService;

    public DeleteTicketCommandHandler(ILogger<DeleteTicketCommandHandler> logger, ITicketService ticketService)
    {
        _logger = logger;
        _ticketService = ticketService;
    }

    public async Task<ServiceResponse<Guid>> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var ticketId = await _ticketService.DeleteTicket(request.Id, cancellationToken);

        if (ticketId == Guid.Empty)
        {
            _logger.LogWarning("Route with ID {TicketId} not found for deletion.", request.Id);
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Không tìm thấy vé!",
                Data = Guid.Empty
            };
        }

        _logger.LogInformation("Delete updated with ID: {TicketId}", ticketId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Xóa vé thành công!",
            Data = ticketId
        };

    }
}
