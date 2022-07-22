using PXS.IfRF.Data;
using PXS.IfRF.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using PXS.IfRF.Logging;
using d=System.Diagnostics.Debug;
using System.Reflection;

namespace PXS.IfRF.Services
{
    public class RefdataService : IRefdataService
    {
        private readonly ModelContext	_ifrfContext;
		private readonly ILoggerManager _logger;

        public RefdataService(ModelContext ifrfContext, ILoggerManager logger)
        {
            _ifrfContext	= ifrfContext;
			_logger         = logger;
        }

        /// <summary date="11-06-2021, 09:29:58" author="S.Deckers">
        /// Read item from db
        /// </summary>
        public RefPl GetRefdata(string refdataType, string key)
        {
            var propInfo = _ifrfContext.GetType().GetProperties().SingleOrDefault(m => m.Name.Equals(refdataType));
            if (propInfo == null) return null;

            IQueryable<RefPl> refDatas = propInfo.GetValue(_ifrfContext) as IQueryable<RefPl>;

			RefPl rd = refDatas.SingleOrDefault(a => a.Key == key);
            return rd;
        }

        /// <summary date="11-06-2021, 09:19:39" author="S.Deckers">
        /// Read items from db
        /// </summary>
        public IQueryable<RefPl> GetRefdatas(string refdataType)
        {
			_logger.Debug(refdataType);

			var propInfo = _ifrfContext.GetType().GetProperties().SingleOrDefault(m => m.Name.Equals(refdataType));

			if (propInfo == null) return Enumerable.Empty<RefPl>().AsQueryable();

            if (!(propInfo.GetValue(_ifrfContext) is IQueryable<RefPl> refDatas))
            {
                return Enumerable.Empty<RefPl>().AsQueryable();
            }

            IList<RefPl> castedRefDatas = new List<RefPl>();
            foreach (RefPl aRefData in refDatas)
            {
				castedRefDatas.Add(aRefData);
            }

            return castedRefDatas.AsQueryable();
        }
    }
}