using Shared;
using Shared.QueryParameter;

namespace Application.DTOs.Commons;

public class InvoiceQueryParameters : CommonQueryParameters
{
    public Guid? BoothId { get; set; }
    public string? BranchCode { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public PaymentMethodStatus? PaymentMethod { get; set; }
    public int? VoucherId { get; set; }
    public override GenericQueryParameters ToGenericQueryParameters()
    {
        var iqp = base.ToGenericQueryParameters();
        iqp.RemoveFilter("BranchId");
        if (BranchId.HasValue)      iqp.AddFilter("Booth.BranchId", "==", BranchId.Value);
        if (BranchCode != null)     iqp.AddFilter("Booth.Branch.BranchCode", "==", BranchCode);
        if (BoothId.HasValue) iqp.AddFilter("BoothId", "==", BoothId);
        if (From.HasValue) iqp.AddFilter("CreatedAt", ">=", From.Value.Date);
        if (To.HasValue) iqp.AddFilter("CreatedAt", "<=", To.Value.Date.AddDays(1).AddTicks(-1));
        if (PaymentMethod.HasValue) iqp.AddFilter("PaymentMethod", "==", PaymentMethod);
        return iqp;
    }
}