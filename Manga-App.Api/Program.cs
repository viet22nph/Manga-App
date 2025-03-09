using MangaApp.Api.DependencyInjection.Extensions;
using MangaApp.Application.DependencyInjections.Extensions;
using MangaApp.Persistence;
using MangaApp.Persistence.DependencyInjections.Extentions;
using MangaApp.Persistence.SeedData;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<SeedData>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddControllers()
    .AddApplicationPart(MangaApp.Presentation.AssemblyReference.Assembly)
    .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); ;

builder.Services.AddSqlServer();
builder.Services.AddIdentity();

builder.Services.AddCacheRedis(builder.Configuration);
builder.Services.ConfigureSqlServerRetryOption(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddMediatRApplication();
builder.Services.AddAws(builder.Configuration);
builder.Services.AddServicePersitence();
builder.Services.AddQuartzJob();

builder.Services.AddMasstransitConfigurationRabbitMq(builder.Configuration);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
             policy.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
builder.Services.AddApiVersioning(o => {
    o.ReportApiVersions = true;
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
});
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwagger();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.ConfigureSwagger();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.UseStaticFiles();

app.UseCors(MyAllowSpecificOrigins).UseForwardedHeaders();
app.MapControllers();
//Seed
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    var seed = services.GetService<SeedData>();
    if (seed != null)
    {
        await seed.SeedDataAsync();
    }
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();
