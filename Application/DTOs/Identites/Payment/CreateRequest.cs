namespace Application.DTOs.Identites.Payment;

public class CreateRequest
{
    public required string pcode { get; set; }
    public required string merchant_code { get; set; }
    public required string data { get; set; }
}