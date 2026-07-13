using TicketingApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services as singletons (in-memory storage)
builder.Services.AddSingleton<ITicketService, TicketService>();
builder.Services.AddSingleton<IImportService, ImportService>();
builder.Services.AddSingleton<IClassificationService, ClassificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
