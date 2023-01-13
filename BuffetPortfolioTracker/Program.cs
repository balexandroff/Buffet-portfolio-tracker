using BuffetPortfolioTracker.Interfaces;
using BuffetPortfolioTracker.Services;
using BuffetPortfolioTracker.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add configuration options.
builder.Services.Configure<Configuration>(builder.Configuration.GetSection(Configuration.AppSettings));

// Add services to the container.
builder.Services.AddTransient<IPortfolioService, PortfolioService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
