using Microsoft.AspNet.OData.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PXS.IfRF.Logging
{
    public static class StructuralTypeConfigExtension
    {
        /// <summary>
        /// This method ensures that all primitive types in the mapped classes are part of the model
        /// Also those marked as "Ignore" for not having a backing field in the DB
        /// </summary>
        /// <param name="s"></param>
        public static void AddAllProperties(this StructuralTypeConfiguration s)
        {
            foreach (PropertyInfo p in s.ClrType.GetProperties())
            {
                if (p.GetAccessors().Any(a => !a.IsVirtual))
                {
                    s.AddProperty(p);
                }
                else if (p.CustomAttributes.Any(a => a.AttributeType.Name.Equals("NotMappedAttribute")))
                {
                    s.AddProperty(p);
                }
            }
        }
    }
}
