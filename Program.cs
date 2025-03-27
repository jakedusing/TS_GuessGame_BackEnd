var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register SongService as a service so it can be injected
builder.Services.AddSingleton<SongService>();

var app = builder.Build();


app.UseHttpsRedirection();

app.MapControllers();

app.Run();