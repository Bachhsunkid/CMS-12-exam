using Microsoft.AspNetCore.Mvc;
using TrainingTest.Business;

namespace TrainingTest.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTrainingTest(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(options => options.Filters.Add<PageContextActionFilter>());
        
        return services;
    }
}