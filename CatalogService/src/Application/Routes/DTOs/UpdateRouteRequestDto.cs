using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.Routes.DTOs;

public class UpdateRouteRequestDto
{
    [Required(ErrorMessage = "Xin vui lòng nhập code.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Code yêu cầu 6 chữ số.")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "Xin vui lòng nhập tên tuyến.")]
    public string Name { get; set; } = null!;

    [MaxLength(200, ErrorMessage = "Đường dẫn ảnh không được vượt quá 256 ký tự.")]
    public string? ThumbnailImageUrl { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Chiều dài tuyến phải lớn hơn 0.")]
    public double LengthInKm { get; set; }
}
