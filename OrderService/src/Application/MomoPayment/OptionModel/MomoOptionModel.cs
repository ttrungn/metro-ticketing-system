using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.OptionModel;

public class MomoOptionModel
{

    [Required] public string? PartnerCode { get; set; }

    [Required] public string? AccessKey { get; set; } 

    [Required] public string? SecretKey { get; set; } 

    [Required] public string? PaymentUrl { get; set; }
    
    [Required] public string? RedirectUrl { get; set; }

    [Required] public string? IpnUrl { get; set; }

    [Required] public string? RequestType { get; set; }    

}
