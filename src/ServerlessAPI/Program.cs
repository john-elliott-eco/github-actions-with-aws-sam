using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ServerlessAPI.Entities;
using ServerlessAPI.Repositories;
using ServerlessAPI.Services;
using Tomlyn;
using Tomlyn.Model;


var builder = WebApplication.CreateBuilder(args);

//Logger
builder.Logging
        .ClearProviders()
        .AddJsonConsole();
 
// Add services to the container.
builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

string region = Environment.GetEnvironmentVariable("AWS_REGION") ?? RegionEndpoint.USEast2.SystemName;
builder.Services
        .AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(RegionEndpoint.GetBySystemName(region)))
        .AddScoped<IDynamoDBContext, DynamoDBContext>()
        .AddScoped<IBookRepository, BookRepository>();

// Add AWS Lambda support. When running the application as an AWS Serverless application, Kestrel is replaced
// with a Lambda function contained in the Amazon.Lambda.AspNetCoreServer package, which marshals the request into the ASP.NET Core hosting framework.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);


var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda - Test");

app.MapGet("/test", async () =>
{
    //var getParams = new GetParams();
    //var newQueueName = await getParams.GetParameterAsync("/sypol/owner-notifications/Dev/secret-test", false);
    //return $"New Queue Name: {newQueueName}";

    var getParams = new GetParams();
    var parameters = await getParams.GetParametersAsync("/sypol/owner-notification/secret-test", true);
    return string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}"));
});

app.MapGet("/config", (IConfiguration configuration) =>
{
    // Retrieve the value of 'jeenv' from appsettings.json
    var jeenv = configuration["jeenv"];

    // Retrieve the value of 'samenv' from the environment variable
    var samenv = Environment.GetEnvironmentVariable("SAMENV");

    // Return both values
    return new
    {
        jeenv = jeenv ?? "jeenv not found",
        samenv = samenv ?? "samenv not found"
    };
});


app.Run();
