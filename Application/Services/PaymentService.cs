using Application.DTOs.Identites.Payment;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Results;

namespace Application.Services;

public class PaymentService : GenericService<Payment, PaymentDto, CreatePaymentDto, int>, IPaymentService
{
    private readonly IGenericRepository<Booths, Guid> _boothRepository;
    private readonly IEPAYClientService _epayClient;
    private readonly EpayOptions _options;
    Guid BOOTH_ID = new Guid("c4fe7df1-7f24-4f3d-602d-08df42655ea0");
    public PaymentService(
        IGenericRepository<Payment, int> repository,
        IGenericRepository<Booths, Guid> boothRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IOptions<EpayOptions> options,
        IEPAYClientService ePAYClient
    ) : base(repository, mapper, unitOfWork)
    {
        _boothRepository = boothRepository;
        _options = options.Value;
        _epayClient = ePAYClient;
    }

    public override async Task<ServiceResult<PaymentDto>> CreateAsync(CreatePaymentDto dto)
    {
        try
        {
            var payment = new Payment
            {
                BoothId = BOOTH_ID,
                Amount = dto.Amount,
                Status = PaymentStatus.Created
            };
            _repository.Add(payment);
            await _unitOfWork.SaveChangesAsync();

            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = startDate.AddMinutes(15);
            var data_regis = new CreateRegisterReqDto
            {
                map_id = GenRegisterMapId(BOOTH_ID),
                amount = dto.Amount,
                start_date = startDate.ToString("yyMMdd_HHmmss"),
                end_date = endDate.ToString("yyMMdd_HHmmss"),
                condition = "03",
                customer_name = _options.CustomerName,
                request_id = GenRequestId(_options.MerchantCode, BOOTH_ID),
                bank_code = "WOORIBANK"
            };

            var registerResult = await _epayClient.RegisterAsync(data_regis);

            if (!registerResult.IsSuccess || registerResult.Data!.response_code != "00")
            {
                payment.Status = PaymentStatus.Failed;
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<PaymentDto>.InternalServerError(
                    registerResult.Data?.message ?? "Đăng ký DC thất bại");
            }

            payment.account_no = registerResult.Data.account_no;
            payment.qr_rawdata = registerResult.Data.qr_dataRaw;
            payment.map_id = data_regis.map_id;
            payment.Status = PaymentStatus.Pending;
            await _unitOfWork.SaveChangesAsync();

            var result = new PaymentDto
            {
                Amount = payment.Amount,
                Status = payment.Status,
                QR = payment.qr_rawdata
            };
            return ServiceResult<PaymentDto>.Success(result);
        }
        catch(Exception ex)
        {
            var result = new PaymentDto
            {
                Status = PaymentStatus.Failed,
                QR = ""
            };
            return ServiceResult<PaymentDto>.InternalServerError($"Lỗi tạo payment {ex}");
        }
    }

    private string GenRegisterMapId(Guid boothId)
    {
        string mapId = $"{boothId}_{DateTime.Now:yyMMdd_HHmmss}";

        if (mapId.Length > 50)
        {
            Console.WriteLine(mapId.Length);
            throw new ArgumentException("Invalid map_id length");
        }

        return mapId;
    }
    private string GenRequestId(string merchant_code, Guid boothId)
    {
        string request_id = $"{merchant_code}_{boothId}_{DateTime.Now:HHmmss}";
        if(request_id.Length > 50){
            Console.WriteLine(request_id.Length);
            throw new ArgumentException("Invalid request_id length");
        }
        return request_id;
    }
}