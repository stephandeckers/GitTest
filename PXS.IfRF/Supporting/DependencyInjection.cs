using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PXS.IfRF.AuthHandling;
using PXS.IfRF.BusinessLogic;
using PXS.IfRF.Data;
using PXS.IfRF.Services;

namespace PXS.IfRF.Logging
{
	public static class DependencyInjection
	{
		public static void RegisterDiContainers(this IServiceCollection services)
		{
			services.AddSingleton<IHttpContextAccessor,					HttpContextAccessor>();
			services.AddSingleton<IAuthorizationHandler,				CommonAuthorizationHandler>();
			services.AddScoped<IAvailabilityService,					AvailabilityService>();
			services.AddScoped<IFreeConsecutivePositionSearchService,	FreeConsecutivePositionSearchService>();
			services.AddScoped<IFreeSplitterPositionSearchService,		FreeSplitterPositionSearchService>();
			services.AddScoped<IONTPPositionConnectionSearchService,	ONTPPositionConnectionSearchService>();
			services.AddScoped<ISrPositionAggregateService,				SrPositionAggregateService>();
			services.AddScoped<IGlobalPositionAggregateService,			GlobalPositionAggregateService>();
			services.AddScoped<IConnectionService,						ConnectionService>();
			services.AddScoped<IMetaSpecSubrackService,					MetaSpecSubrackService>();
			services.AddScoped<IPopService,								PopService>();
			services.AddScoped<IInfoService,							InfoService>();
			services.AddScoped<IRefdataService,							RefdataService>();
			services.AddScoped<IResourceCharacteristicService,			ResourceCharacteristicService>();
			services.AddScoped<IResourceOrderService,					ResourceOrderService>();
			services.AddScoped<IResourceService,						ResourceService>();
			services.AddScoped<IRfAreaService,							RfAreaService>();
			services.AddScoped<ISrPositionService,						SrPositionService>();
			services.AddScoped<ISubrackService,							SubrackService>();
			services.AddScoped<IRackspaceService,						RackspaceService>();
			services.AddScoped<ISharedMethods,							SharedMethods>();
			services.AddScoped<ISettingsService,						SettingsService>();
			services.AddScoped<IImportService,							ImportService>();
			services.AddSingleton<DbCommandInterceptor,					QueryCommandInterceptor>();
		}
	}
}