using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Application.MomoPayment.Commands.CreateMomoPayment;
using OrderService.Application.MomoPayment.DTOs;

namespace OrderService.Application.Common.Interfaces.Services;
public interface IMomoService
{
    Task<MomoCreatePaymentResponseModel> CreatePaymentWithMomo(CreateMomoPaymentCommand command, CancellationToken cancellationToken = default);
}
