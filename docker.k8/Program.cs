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
var url = Environment.GetEnvironmentVariable("URL");
Console.WriteLine(url);
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

app.MapGet("/weatherforecast_v1", async (ILogger<Program> logger) =>
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

app.MapGet("/GetFileContent", () =>
{
    try
    {
        Console.WriteLine(Environment.CurrentDirectory);
        string contents = File.ReadAllText("./Data/temp.txt");
        return Results.Ok(contents); // Explicit return of result
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception while Get File Content : {ex.Message}");
        return Results.Problem($"Error reading file: {ex.Message}"); // Returning an error result
    }
})
.WithName("GetFileContent")
.WithOpenApi();


app.MapGet("/SetFile", static (string Content) =>
{
    try
    {
        Console.WriteLine(Environment.CurrentDirectory);
        if (!Directory.Exists("./Data"))
        {
            Console.WriteLine("Creating Directory");
            Directory.CreateDirectory("./Data");
        }

        if (!File.Exists("./Data/temp.txt"))
        {
            Console.WriteLine("Creating file");
            using (File.Create("./Data/temp.txt")) { } // This ensures the file stream is properly closed
        }
        Console.WriteLine(Environment.CurrentDirectory);
        Console.WriteLine("Appending to File");
        File.AppendAllText("./Data/temp.txt", Content + Environment.NewLine);
        return Results.Ok(true); // Explicit success response
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception while SetFile: {ex.Message}");
        return Results.Problem($"Error setting file content: {ex.Message}");
    }
})
.WithName("SetFile")
.WithOpenApi();



app.MapGet("/Error", () =>
{
    try
    {
        Console.WriteLine($"restarting the pod.");
        Environment.Exit(1);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception while SetFile: {ex.Message}");
    }
})
.WithName("Error")
.WithOpenApi();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
