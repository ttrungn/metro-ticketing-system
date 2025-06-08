using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Stations.Commands.CreateStation;

public record CreateStationCommand : IRequest<ServiceResponse<Guid>>
{
    public string Name { get; init; } = null!;
    public string? StreetNumber { get; init; }
    public string? Street { get; init; }
    public string? Ward { get; init; }
    public string? District { get; init; }
    public string? City { get; init; }
    public string? ThumbnailImageUrl { get; init; }
}

public class CreateStationCommandValidator : AbstractValidator<CreateStationCommand>
{
    public CreateStationCommandValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty().WithMessage("Xin vui lòng nhập tên trạm!");

        RuleFor(s => s.StreetNumber)
            .MaximumLength(50).WithMessage("Số nhà không được vượt quá 50 ký tự!");

        RuleFor(s => s.Street)
            .MaximumLength(256).WithMessage("Tên đường không được vượt quá 256 ký tự!");

        RuleFor(s => s.Ward)
            .MaximumLength(256).WithMessage("Phường/Xã không được vượt quá 256 ký tự!");

        RuleFor(s => s.District)
            .MaximumLength(256).WithMessage("Quận/Huyện không được vượt quá 256 ký tự!");

        RuleFor(s => s.City)
            .MaximumLength(256).WithMessage("Tỉnh/Thành phố không được vượt quá 256 ký tự!");

        RuleFor(c => c.ThumbnailImageUrl)
            .MaximumLength(256).WithMessage("Đường dẫn ảnh không được vượt quá 256 ký tự!");
    }
}

public class CreateStationCommandHandler : IRequestHandler<CreateStationCommand, ServiceResponse<Guid>>
{
    private readonly IStationService _stationService;
    private readonly ILogger<CreateStationCommandHandler> _logger;

    public CreateStationCommandHandler(ILogger<CreateStationCommandHandler> logger, IStationService stationService)
    {
        _logger = logger;
        _stationService = stationService;
    }

    public async Task<ServiceResponse<Guid>> Handle(CreateStationCommand command, CancellationToken cancellationToken)
    {
        var stationId = await _stationService.CreateAsync(command, cancellationToken);

        _logger.LogInformation("Station created with ID: {StationId}", stationId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Tạo trạm thành công!",
            Data = stationId
        };
    }
}
