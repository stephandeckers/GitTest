/**
 * @Name MinimalApi.cs
 * @Purpose 
 * @Date 06 January 2022, 20:29:19
 * @Author S.Deckers
 * @Description 
 * @Url https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// --- Map ports
//app.Urls.Add("http://localhost:3000");
//app.Urls.Add("http://localhost:4000");

app.MapGet("/", () => "Hello World!");

app.Run();