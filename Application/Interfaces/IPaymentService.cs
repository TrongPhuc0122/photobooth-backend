using Application.DTOs.Identites.Payment;
using Application.Interfaces.Commons;
using Domain.Entities;

namespace Application.Interfaces;

public interface IPaymentService : IGenericService<Payment, PaymentDto, CreatePaymentDto, int>
{
    
}