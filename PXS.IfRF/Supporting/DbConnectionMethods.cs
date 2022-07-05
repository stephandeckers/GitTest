using Microsoft.EntityFrameworkCore;
using PXS.IfRF.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace PXS.IfRF.Supporting
{
    public static class DbConnectionMethods
    {
        public static DbConnection GetOpenConnection (this ModelContext context)
        {
            DbConnection _connection = context.Database.GetDbConnection();
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            return _connection;
        }
    }
}
