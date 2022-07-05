using PXS.IfRF.Data.Model;
using System.Collections.Generic;

namespace PXS.IfRF.Services
{
    public interface ISrPositionAggregateService
    {
        List<SrPositionAggregateResponse> GenerateSrPositionAggregate(long subrackId);
    }
}