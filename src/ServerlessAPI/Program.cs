using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ServerlessAPI.Entities;
using ServerlessAPI.Repositories;


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

app.MapGet("/test", () =>
{
    var books = new List<Book> {
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Book 1",
                ISBN = "1234567890",
                Authors = new List<string> { "Author 1", "Author 2" },
                CoverPage = "https://example.com/cover.jpg"
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Book 2",
                ISBN = "0987654321",
                Authors = new List<string> { "Author 3", "Author 4" },
                CoverPage = "https://example.com/cover2.jpg"
            }
        };

    return books;
});


app.Run();
