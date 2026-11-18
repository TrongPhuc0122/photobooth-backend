namespace Application.DTOs.Identites.Payment;

public class EpayOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string MerchantCode { get; set; } = string.Empty;
    public string Key3Des { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}