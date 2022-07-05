using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PXS.IfRF.Controllers
{
    #region -- Using directives --
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNet.OData;
    using Swashbuckle.AspNetCore.Annotations;
    using PXS.IfRF.Services;
    using PXS.IfRF.Logging;
    using PXS.IfRF.Data.Model;
    using d = System.Diagnostics.Debug;
    using Microsoft.AspNetCore.Http;
    #endregion

    public partial class MetaSpecSubrackController : IfRFBaseController
    {
        private const string swaggerControllerDescription = "Subrack Specifications";
        private readonly IMetaSpecSubrackService _metaSpecSubrackService;

        public MetaSpecSubrackController(
            IMetaSpecSubrackService metaSpecSubrackService,
            ILoggerManager logger,
            IRefdataService refdataService)
            : base(logger, refdataService)
        {
            _metaSpecSubrackService = metaSpecSubrackService;

        }

        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 4)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list of Subrack Specifications")]
        public IQueryable<MetaSpecSubrack> GetItems()
        {
            var localItems = _metaSpecSubrackService.GetItems();
            return localItems;
        }
    }
}
