using Shared;
using Shared.QueryParameter;

namespace Application.DTOs.Commons;

public class VoucherQueryParameters : CommonQueryParameters
{
    public UsedStatus? BeUsed { get; set; }
    public override GenericQueryParameters ToGenericQueryParameters()
    {
        var gqp = base.ToGenericQueryParameters();
        if (BeUsed.HasValue)
        {
            var now = DateTime.UtcNow;
            switch (BeUsed)
            {
                case UsedStatus.UpComing:
                    gqp.AddFilter("StartDate", ">", now);
                    break;
                case UsedStatus.Active:
                    gqp.AddFilter("StartDate", "<=", now);
                    gqp.AddFilter("EndDate", ">=", now);
                    break;
                case UsedStatus.Expired:
                    gqp.AddFilter("EndDate", "<", now);
                    break;
            }
        }
        Console.WriteLine($"[DEBUG] usedStatus={BeUsed}, FilterCount={gqp.Filters.Count}");
        foreach (var f in gqp.Filters)
        {
            Console.WriteLine($"[DEBUG] Field={f.Field}, Operator='{f.Operator}', Value={f.Value}");
        }
        return gqp;
    }
}