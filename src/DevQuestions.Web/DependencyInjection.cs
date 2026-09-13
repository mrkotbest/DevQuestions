using DevQuestions.Application;
using DevQuestions.Infrastructure.PostgreSql;

namespace DevQuestions.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services)
    {
        services.AddWeb();
        services.AddApplication();
        services.AddPostgreSqlInfrastructure();

        return services;
    }

    private static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}
