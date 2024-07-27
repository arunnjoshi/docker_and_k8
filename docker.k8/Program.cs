using Serilog;
var builder = WebApplication.CreateBuilder(args);


// Configure Serilog before configuring services
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


var port = Environment.GetEnvironmentVariable("PORT");
Console.WriteLine($"Current PORT: {port}");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");
}

// builder.WebHost.ConfigureKestrel((context, options) =>
// {
//     options.Configure(context.Configuration.GetSection("Kestrel"));
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (ILogger<Program> logger) =>
{
    var http = new HttpClient();
    var responseMessage = await http.GetAsync("https://jsonplaceholder.typicode.com/todos");
    if (responseMessage.IsSuccessStatusCode)
    {
        var res = await responseMessage.Content.ReadAsStringAsync();
        logger.LogInformation("Get Req from {url} and {res}", "https://jsonplaceholder.typicode.com/todos", res);
        return res;
    }

    return string.Empty;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
