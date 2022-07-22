using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using PXS.IfRF.Logging;

namespace PXS.IfRF.Logging
{
    /// <summary>
    /// Swagger customization to add query attributes like $select and $filter to those methods that support them
    /// </summary>
    public class SwaggerQueryParameter : IOperationFilter
    {
        public void Apply( OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            var attr = context.MethodInfo.CustomAttributes.ToList();

            if (attr.Any(a => a.AttributeType == typeof(EnableQueryAttribute)))
            {
                OpenApiSchema stringSchema = new OpenApiSchema { Type = "string" };

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "$select",
                    In = ParameterLocation.Query,
                    Description = "Select specific fields, comma separated, e.g. $select=name,type",
                    Required = false,
                    Schema = stringSchema,
                    AllowReserved = true
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "$orderby",
                    In = ParameterLocation.Query,
                    Description = "Order records by a field asc or descending e.g. $orderby=name,type desc",
                    Required = false,
                    Schema = stringSchema,
                    AllowReserved = true
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "$count",
                    In = ParameterLocation.Query,
                    Description = "Count the number of records. e.g. $count=true. To have only the count number, include $top=0",
                    Required = false,
                    Schema = stringSchema,
                    AllowReserved = true
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "$top",
                    In = ParameterLocation.Query,
                    Description = "only return the n top records. e.g. $top=5",
                    Required = false,
                    Schema = stringSchema,
                    AllowReserved = true
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "$skip",
                    In = ParameterLocation.Query,
                    Description = "Skip the first n records. e.g $skip=10",
                    Required = false,
                    Schema = stringSchema,
                    AllowReserved = true
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "$filter",
                    In = ParameterLocation.Query,
                    Description = "Advanced filtering options. e.g. $filter=indexof(toupper(RfAreaCode),'AR') eq 0",
                    Required = false,
                    Schema = stringSchema,
                    AllowReserved = true
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "$search",
                    In = ParameterLocation.Query,
                    Description = "Only return records matching a specific search expression. e.g. $search=sometext",
                    Required = false,
                    Schema = stringSchema,
                    AllowReserved = true
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "$expand",
                    In = ParameterLocation.Query,
                    Description = "Expands related entities. Generally those names with end with 'Ref'. e.g. $expand=OwnerRef,ZoneRef",
                    Required = false,
                    Schema = stringSchema,
                    AllowReserved = true
                });
            }
        }
    }
}