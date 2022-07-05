
using Microsoft.AspNet.OData;
using PXS.IfRF.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace PXS.IfRF.Services
{
    public interface IRefdataService
    {
        RefPl				GetRefdata	( string refdataType, string key);
        IQueryable<RefPl>	GetRefdatas	( string refdataType);
    }
}

