using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Tickets.DTO;
using CatalogService.Domain.Enum;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Tickets.Queries.GetTickets;

public record GetTicketsQuery : IRequest<ServiceResponse<GetTicketsResponseDto>>
{
    public int Page { get; init; } = 0;
    public int PageSize { get; init; } = 8;
    public string? Name { get; set; } = string.Empty;
    public decimal? MinPrice { get; set; } = Decimal.MinValue;
    public decimal? MaxPrice { get; set; } = Decimal.MaxValue;
    public TicketTypeEnum? TicketType { get; set; }
    public bool? Status { get; init; } = false;
}

public class GetTicketsQueryValidator : AbstractValidator<GetTicketsQuery>
{
    public GetTicketsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(0).WithMessage("Số trang phải lớn hơn 0!");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Kích thước trang phải lớn hơn 0!");
    }
}

public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, ServiceResponse<GetTicketsResponseDto>>
{
    private readonly ILogger<GetTicketsQueryHandler> _logger;
    private readonly ITicketService _ticketService;

    public GetTicketsQueryHandler(ILogger<GetTicketsQueryHandler> logger, ITicketService ticketService)
    {
        _logger = logger;
        _ticketService = ticketService;
    }

    public async Task<ServiceResponse<GetTicketsResponseDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        var (tickets, totalPages) = await _ticketService.GetTickets(request, cancellationToken);

        var response = new GetTicketsResponseDto()
        {
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            Tickets = tickets,
        };

        _logger.LogInformation("Retrieve tickets successfully: Total pages: {TotalPages} - Current page: {CurrentPage} - Page size: {PageSize}",
            response.TotalPages, response.CurrentPage, response.PageSize);
        return new ServiceResponse<GetTicketsResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy danh sách vé thành công!",
            Data = response
        };
    }
}
