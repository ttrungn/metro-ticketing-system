using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Application.MomoPayment.Commands.CreateMomoPayment;
using OrderService.Application.MomoPayment.DTOs;
using OrderService.Application.OptionModel;

namespace OrderService.Infrastructure.Services;
public class MomoService : IMomoService
{
    private readonly ILogger<MomoService> _logger;

    private readonly HttpClient _client;

    private readonly IOptions<MomoOptionModel> _options;
    public MomoService(ILogger<MomoService> logger, HttpClient client, IOptions<MomoOptionModel> options)
    {
        _logger = logger;
        _client = client;
        _options = options;
    }
    public async Task<MomoCreatePaymentResponseModel> CreatePaymentWithMomo(CreateMomoPaymentCommand command, CancellationToken cancellationToken = default)
    {
        var options = _options.Value;
        var momoRequest = command.ToMomoOneTimePayment(options);


        var requestJson = JsonSerializer.Serialize(momoRequest);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(options.PaymentUrl, content);
        var responseJson = await response.Content.ReadAsStringAsync();

        var momoResponse = JsonSerializer.Deserialize<MomoCreatePaymentResponseModel>(responseJson);

        return momoResponse!;
    }
}
