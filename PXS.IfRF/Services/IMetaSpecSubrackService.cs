using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PXS.IfRF.Services
{
    #region -- Using directives --
    using Microsoft.AspNet.OData;
    using Microsoft.AspNetCore.Http;
    using PXS.IfRF.Data.Model;
    using System.Collections.Generic;
    using System.Linq;
    #endregion

    public interface IMetaSpecSubrackService
    {
        IQueryable<MetaSpecSubrack> GetItems();
    }
}
