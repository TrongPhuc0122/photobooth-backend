using Shared;
using Shared.QueryParameter;

namespace Application.DTOs.Commons;

public class InvoiceQueryParameters : CommonQueryParameters
{
    public Guid? BoothId { get; set; }
    public string? BranchCode { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public PaymentMethodStatus? PaymentMethod { get; set; }
    public int? VoucherId { get; set; }
    public override GenericQueryParameters ToGenericQueryParameters()
    {
        var iqp = base.ToGenericQueryParameters();
        iqp.RemoveFilter("BranchId");
        if (BranchId.HasValue)      iqp.AddFilter("Booth.BranchId", "==", BranchId.Value);
        if (BranchCode != null)     iqp.AddFilter("Booth.Branch.BranchCode", "==", BranchCode);
        if (BoothId.HasValue) iqp.AddFilter("BoothId", "==", BoothId);
        if (FromDate.HasValue) iqp.AddFilter("CreatedAt", ">=", FromDate.Value.Date);
        if (ToDate.HasValue) iqp.AddFilter("CreatedAt", "<=", ToDate.Value.Date.AddDays(1).AddTicks(-1));
        if (PaymentMethod.HasValue) iqp.AddFilter("PaymentMethod", "==", PaymentMethod);
        return iqp;
    }
}