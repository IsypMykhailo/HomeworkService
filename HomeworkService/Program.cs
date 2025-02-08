using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HomeworkService.Database;
using HomeworkService.Domain.Extensions;
using HomeworkService.Domain.Validators;
using HomeworkService.Middlewares;
using HomeworkService.Repositories;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

var services = builder.Services;

var connectionString = builder.Configuration.GetValue<string>("Database:ConnectionString");
var poolSize = builder.Configuration.GetValue<int>("Database:PoolSize");

services.AddDbContextPool<HomeworkContext>(options =>
{
    options
        .UseNpgsql(connectionString)
        .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
}, poolSize);

services.AddScopedServices();
services.AddScoped(typeof(ICrudRepository<>), typeof(RepositoryBase<>));

services.AddDateOnlyTimeOnlyStringConverters();

services.AddFluentValidationAutoValidation();
services.AddValidatorsFromAssemblyContaining<Program>();

services
    .AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options => options.UseDateOnlyTimeOnlyStringConverters());

var app = builder.Build();

app.UseSerilogRequestLogging();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();