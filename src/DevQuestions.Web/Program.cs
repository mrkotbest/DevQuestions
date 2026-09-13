using DevQuestions.Web;

var builder = WebApplication.CreateBuilder(args);

DependencyInjection.AddProgramDependencies(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DevQuestions API"));
}

app.MapControllers();

app.Run();
