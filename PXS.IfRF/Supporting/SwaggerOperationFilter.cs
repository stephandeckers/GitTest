using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PXS.IfRF.Logging;
using d=System.Diagnostics.Debug;

namespace PXS.IfRF.Logging
{
    /// <summary>
    /// This class is there to support the Delta<> operation in combination with Swagger derived Schemas
    /// Whithout, there's a lot of internal classes exposed as schemas
    /// For some reason, the original implementation copied from internet did not work
    /// 
    /// Additionally it also removes all mediatypes except application/json for the examples section in swagger
    /// </summary>
	//
    public class SwaggerOperationFilter:IOperationFilter
    {
        private const string _deltaParam        = "Delta";
        private const string APPJSON            = "application/json";
        private const string MULTIPART_FORMDATA = "multipart/form-data";

        private readonly IList<string> dataNamespaces;
        public SwaggerOperationFilter()
        {
            dataNamespaces = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(t => t.GetTypes())
				.Where(t => (t.IsClass || t.IsAbstract) && ( t.Namespace == "PXS.IfRF.Data.Model" || t.Namespace == "PXS.IfRF.Data.Model.ONTPPositionConnectionSearch"))
                .Select(n => n.Name).ToList();
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody == null) return;

            RemoveRedundantResponseHeaders(operation);

            RemoveRedundantRequestHeaders(operation);

            var deltaTypes =
                operation.RequestBody
                    .Content
                    .Where(x => x.Value.Schema.Reference != null && x.Value.Schema.Reference.Id.EndsWith(_deltaParam));

            foreach (var (_, value) in deltaTypes)
            {
                var schema = value.Schema;
                string model = schema.Reference.Id.Substring(0, schema.Reference.Id.Length - _deltaParam.Length);
                schema.Reference.Id = model;
            }

            var schemas = context.SchemaRepository.Schemas;

            var redundantSchemas = schemas.Where(s => !dataNamespaces.Contains(s.Key) && !dataNamespaces.Contains(s.Key.Replace("Delta","")));

            foreach (var (key, value) in redundantSchemas)
            {
                schemas.Remove(key);
            }
        }

        private void RemoveRedundantRequestHeaders(OpenApiOperation operation)
        {
            //var backupJson = operation.RequestBody.Content.SingleOrDefault(x => x.Key.Equals(APPJSON));

            KeyValuePair<string, OpenApiMediaType> backupJson       = operation.RequestBody.Content.SingleOrDefault(x => x.Key.Equals( APPJSON));
            KeyValuePair<string, OpenApiMediaType> backupMultiPart  = operation.RequestBody.Content.SingleOrDefault(x => x.Key.Equals( MULTIPART_FORMDATA));

            //if( backupJson.Key != null)
            //{
            //    d.WriteLine( $"{backupJson.Key}.{backupJson.Value}");
            //}

            //multipart/form-data
            operation.RequestBody.Content.Clear();
            //keep multipart type
            //d.WriteLine( );            
            if( backupJson.Key != null)
            {
                operation.RequestBody.Content.Add(backupJson);;
            }            

            if( backupMultiPart.Key != null)
            {
                operation.RequestBody.Content.Add( backupMultiPart);;
            }            
        }

        private void RemoveRedundantResponseHeaders(OpenApiOperation operation)
        {
            OpenApiResponses responses = operation.Responses;
            foreach (var response in responses.Values)
            {
                OpenApiMediaType backupJsonType         = response.Content.FirstOrDefault( c => c.Key.Equals(APPJSON)).Value;

                response.Content.Clear();

                if (backupJsonType != null)
                {
                    response.Content.Add(APPJSON, backupJsonType);
                }
            }
        }
    }
}
