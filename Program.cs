using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Microsoft.Restier.AspNetCore;
using Microsoft.Restier.Core;
using restier_swagger_bug_report;
using restier_swagger_bug_report.Controllers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IgnoreObsoleteActions();
    options.IgnoreObsoleteProperties();
    options.SwaggerDoc("v1"
                     , new OpenApiInfo
                     {
                         Title = "Sample API"
                         ,
                         Version = "2.0.x"
                         ,
                         Description = "Sample API"
                     });
    // Set the comments path for the Swagger JSON and UI.
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
// Workaround: https://github.com/OData/WebApi/issues/1177
SetOutputFormatters(builder.Services);
builder.Services.AddRestier((restierBuilder) =>
{

    // This delegate is executed after OData is added to the container.
    // Add you replacement services here.
    restierBuilder.AddRestierApi<SampleRestierApi>(routeServices =>
    {
        routeServices.AddOptions();
        routeServices.AddSwaggerGen(options =>
        {
            options.IgnoreObsoleteActions();
            options.IgnoreObsoleteProperties();
            options.SwaggerDoc("v1"
                             , new OpenApiInfo
                             {
                                 Title = "Sample API"
                                 ,
                                 Version = "2.0.x"
                                 ,
                                 Description = "Sample API"
                             });
            // Set the comments path for the Swagger JSON and UI.
            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
        // Workaround: https://github.com/OData/WebApi/issues/1177
        SetOutputFormatters(routeServices);
        routeServices.AddEFCoreProviderServices<SampleDbContext>((serviceProvider, options) =>
            options
                .UseInMemoryDatabase("sample")
                .EnableDetailedErrors()
        );

        routeServices.AddSingleton(new ODataValidationSettings
        {
            MaxTop = 100,
            MaxNodeCount = 1500,
            MaxAnyAllExpressionDepth = 5,
            MaxExpansionDepth = 5,
            AllowedFunctions = AllowedFunctions.AllFunctions,
            AllowedLogicalOperators = AllowedLogicalOperators.All,
            AllowedArithmeticOperators = AllowedArithmeticOperators.All,
            AllowedQueryOptions = AllowedQueryOptions.All,
        });
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseRestierBatching();
app.UseMvc(builder =>
{
    builder.Select().Expand().Filter().OrderBy().MaxTop(100).Count().SetTimeZoneInfo(TimeZoneInfo.Utc);
    builder.MapRestier(restierRouteBuilder =>
    {
        // ReSharper disable once RedundantArgumentDefaultValue
        restierRouteBuilder.MapApiRoute<SampleRestierApi>("ApiV1", "odata", true);
    });
});
app.Run();

static void SetOutputFormatters(IServiceCollection services)
{
    services.AddMvcCore(options =>
    {
        foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
        {
            outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));
        }
        foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
        {
            inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));
        }
    });
}
