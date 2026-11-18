namespace Application.DTOs.Identites.Payment;

public class RegisterResDto
{
    public string response_code { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
    public string account_no { get; set; } = string.Empty;
    public string bank_code { get; set; } = string.Empty;
    public string bank_name { get; set; } = string.Empty;
    public string map_id { get; set; } = string.Empty;
    public string qr_dataRaw { get; set; } = string.Empty;
    public string start_date { get; set; } = string.Empty;
    public string end_date { get; set; } = string.Empty;
}