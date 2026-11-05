using System.ComponentModel.DataAnnotations;
using Domain.Entities.Commons;
using Shared;

namespace Domain.Entities;

public class StatusCode : BaseEntity
{
    [Key]
    public required string Code {get; set; }
    public required string Message {get; set; }
    public StatusType Type {get; set; }
    public DateTime CreateAt {get; set;}
}