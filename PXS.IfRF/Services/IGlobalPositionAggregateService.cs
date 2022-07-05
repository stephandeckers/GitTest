using PXS.IfRF.Data.Model;
using System.Collections.Generic;

namespace PXS.IfRF.Services
{
    public interface IGlobalPositionAggregateService
    {
        (List<GlobalPositionAggregateResponse>, string) GenerateGlobalPositionAggregate (GlobalPositionAggregateRequest request);
    }
}