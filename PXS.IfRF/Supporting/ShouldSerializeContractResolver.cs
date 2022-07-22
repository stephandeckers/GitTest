using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PXS.IfRF.Logging
{
    /// <summary>
    /// This contractresolved will eliminate embedded classes from serialization for all requests that are not handled through OData, but directly through EF
    /// OData seems to take care of this by itself
    /// </summary>
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType != typeof(string))
            {
                if (property.PropertyType.GetInterface(nameof(IEnumerable)) != null)
                {
                    property.ShouldSerialize =
                        instance =>
                        {
                            PropertyInfo propInfo = instance?.GetType().GetProperty(property.PropertyName);
                            if (propInfo is null)
                            { 
                                return false; 
                            }
                            else
                            {
                                return (propInfo.GetValue(instance) as IEnumerable<object>)?.Count() > 0;
                            }
                        };
                }
            }

            if (property.PropertyType.IsValueType)
            {
            }
            return property;
        }
    }
}
