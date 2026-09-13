using DevQuestions.Web;
using DevQuestions.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies();

var app = builder.Build();

app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DevQuestions API"));
}

app.MapControllers();

app.Run();
