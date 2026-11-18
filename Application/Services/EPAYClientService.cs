using System.Text.Json;
using Application.DTOs.Identites.Payment;
using Application.Interfaces;
using Shared.Results;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Application.Services;

public class EPAYClientService : IEPAYClientService
{
    private readonly EpayOptions _options;
    private readonly HttpClient _http;

    public EPAYClientService(IOptions<EpayOptions> options, HttpClient http)
    {
        _options = options.Value;
        _http = http;
    }
    public async Task<ServiceResult<RegisterResDto>> RegisterAsync(CreateRegisterReqDto dto)
    {
        if (decimal.Truncate(dto.amount) != dto.amount)
        {
            return ServiceResult<RegisterResDto>.BadRequest("amount phải là số nguyên");
        }

        var plain = JsonSerializer.Serialize(dto);
        var encrypted = Encrypt3DesHex(plain, _options.Key3Des);

        var req = new CreateRequest
        {
            pcode = "9000",
            merchant_code = _options.MerchantCode,
            data = encrypted
        };

        var httpRes = await _http.PostAsJsonAsync(_options.BaseUrl + "registerVA", req);
        var result = await httpRes.Content.ReadFromJsonAsync<RegisterResDto>();

        if (result is null)
        {
            return ServiceResult<RegisterResDto>.InternalServerError("EPAY trả về dữ liệu không đọc được");
        }

        return ServiceResult<RegisterResDto>.Success(result);
    }

    private static string Encrypt3DesHex(string plain, string key)
    {
        if (key.Length != 24)
            throw new ArgumentException("Key phải có 24 ký tự");

        using var des = TripleDES.Create();
        des.Key = Encoding.ASCII.GetBytes(key);
        des.Mode = CipherMode.ECB;
        des.Padding = PaddingMode.PKCS7;

        using var encryptor = des.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plain);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToHexString(cipherBytes).ToLowerInvariant();
    }
}
