using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PXS.IfRF.Logging;

namespace PXS.IfRF.Logging
{
    /// <summary>
    /// This class is there to support the Delta<> operation in combination with Swagger derived Schemas
    /// Whithout, there's a lot of internal classes exposed as schemas
    /// For some reason, the original implementation copied from internet did not work
    /// </summary>
    public class DeltaSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.Name.Contains("Delta"))
            {
                schema.Properties.Clear();
                //context.SchemaRepository.Schemas.Clear();
            }
        }
    }
}
