using BankingApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddSingleton<ITransactionService, TransactionService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
