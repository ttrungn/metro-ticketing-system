using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Application.MomoPayment.Commands.ConfirmMomoPayment;
using OrderService.Application.MomoPayment.Commands.CreateMomoPayment;
using OrderService.Application.MomoPayment.DTOs;
using OrderService.Application.OptionModel;
using OrderService.Application.Common.Mappings;
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

    public Task<bool> ConfirmMomoPaymentAsync(ConfirmMomoPaymentCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Confirming MoMo payment with command: {Command}", JsonConvert.SerializeObject(command, Formatting.Indented));
        var options = _options.Value;
        if (command.IsValidSignature(options) == false)
        {
            _logger.LogError("Invalid signature for MoMo payment confirmation.");
            return Task.FromResult<bool>(false);
        }
        

        return Task.FromResult<bool>(true);
    }

    public async Task<MomoCreatePaymentResponseModel> CreatePaymentWithMomo(CreateMomoPaymentCommand command, CancellationToken cancellationToken = default)
    {
        var options = _options.Value;
        var momoRequest = command.ToMomoOneTimePayment(options);


        var requestJson = JsonConvert.SerializeObject(momoRequest, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
        });
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        _logger.LogInformation("Sending MoMo payment request: {Payload}", requestJson);
        _logger.LogInformation("POST to URL: {Url}", options.PaymentUrl);

        var response = await _client.PostAsync(options.PaymentUrl, content);
       
        var responseJson = await response.Content.ReadAsStringAsync();
        var momoResponse = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(responseJson);
        _logger.LogInformation("MoMo Raw Response: {Response}", responseJson);
        _logger.LogInformation("MoMo Status Code: {StatusCode}", response.StatusCode);

        return momoResponse!;



    }
}
