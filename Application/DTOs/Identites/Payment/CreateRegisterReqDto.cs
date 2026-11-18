using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identites.Payment;

public class CreateRegisterReqDto
{
    [MaxLength(50)]
    public required string map_id { get; set; }  

    public required decimal amount { get; set; }

    [MaxLength(14)]
    public required string start_date { get; set; }  

    [MaxLength(14)]
    public required string end_date { get; set; }  

    [MaxLength(2)]
    public required string condition { get; set; }  

    [MaxLength(50)]
    public required string customer_name { get; set; }  

    [MaxLength(50)]
    public required string request_id { get; set; }  

    [MaxLength(50)]
    public required string bank_code { get; set; }
}