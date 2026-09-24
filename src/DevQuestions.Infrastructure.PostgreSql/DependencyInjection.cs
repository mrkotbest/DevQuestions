using DevQuestions.Application.Questions;
using DevQuestions.Infrastructure.PostgreSql.Questions;
using Microsoft.Extensions.DependencyInjection;

namespace DevQuestions.Infrastructure.PostgreSql;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgreSqlInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IQuestionsRepository, QuestionsEfCoreRepository>();

        services.AddDbContext<QuestionsDbContext>();

        return services;
    }
}
