using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PXS.IfRF.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PXS.IfRF.ErrorHandling
{
	/// <summary>
	/// OData by default does nice validations but does not return the validation result details. 
	/// Here we extract them and create our own response object in case of errors
	/// </summary>
	public static class InvalidModelStateResponseGenerator
	{
		public static IActionResult GenerateInvalidModelStateResponse(ActionContext context, ILoggerManager logger)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var modelStateNode in context.ModelState.Values)
			{
				foreach (var modelStateError in modelStateNode.Errors)
				{
					string message = modelStateError.Exception != null ? modelStateError.Exception.Message : modelStateError.ErrorMessage;
					// someway odata adds clutter that we want to remove
					string stringToRemove = "One or more errors occurred. (";
					while (message.StartsWith(stringToRemove))
					{
						message = message.Substring(stringToRemove.Length, message.Length - stringToRemove.Length - 1);
					}
					if (sb.Length > 0)
					{
						sb.Append(";");
					}
					sb.Append(message);
				}
			}
			var problemDetails = new ValidationProblemDetails(context.ModelState)
			{
				Instance = context.HttpContext.Request.Path,
				Status = StatusCodes.Status400BadRequest,
				Type = "https://asp.net/core",
				Detail = sb.ToString()
			};
			logger.Error(problemDetails.Title + Environment.NewLine + problemDetails.Detail);
			return new BadRequestObjectResult(problemDetails)
			{
				ContentTypes = { "application/problem+json", "application/problem+xml" }
			};
		}
	}
}
