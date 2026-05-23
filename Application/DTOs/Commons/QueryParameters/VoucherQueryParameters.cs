using Shared;
using Shared.QueryParameter;

namespace Application.DTOs.Commons;

public class VoucherQueryParameters : CommonQueryParameters
{
    public UsedStatus? usedStatus { get; set; }
    public override GenericQueryParameters ToGenericQueryParameters()
    {
        var gqp = base.ToGenericQueryParameters();
        if (usedStatus.HasValue)
        {
            var now = DateTime.UtcNow;
            switch (usedStatus)
            {
                case UsedStatus.UpComing:
                    gqp.AddFilter("StartDate", ">", now);
                    break;
                case UsedStatus.Active:
                    gqp.AddFilter("StartDate", "<=", now);
                    gqp.AddFilter("EndDate", ">=    ", now);
                    break;
                case UsedStatus.Expired:
                    gqp.AddFilter("EndDate", "<", now);
                    break;
            }
        }
        return gqp;
    }
}