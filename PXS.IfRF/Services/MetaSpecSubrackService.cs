using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using PXS.IfRF.AuthHandling;
using PXS.IfRF.Data.Model;
using PXS.IfRF.Logging;
using System;
using System.Linq;


namespace PXS.IfRF.Services
{
    public class MetaSpecSubrackService : IMetaSpecSubrackService
    {
        private readonly ModelContext _ifrfContext = null;
        private readonly ILoggerManager _logger = null;

        public MetaSpecSubrackService(ModelContext ifrfContext, ILoggerManager logger)
        {
            _ifrfContext = ifrfContext;
            _logger = logger;
        }
        /// <summary date="11-06-2021, 09:19:39" author="S.Deckers">
        /// Read items from db
        /// </summary>
        public IQueryable<MetaSpecSubrack> GetItems()
        {
            return _ifrfContext.MetaSpecSubracks;
        }
    }
}
