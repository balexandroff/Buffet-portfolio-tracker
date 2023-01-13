using BuffetPortfolioTracker.BackgrounJobs;
using BuffetPortfolioTracker.Interfaces;
using BuffetPortfolioTracker.Services;
using BuffetPortfolioTracker.Utils;
using Quartz;
using static Quartz.Logging.OperationName;

var builder = WebApplication.CreateBuilder(args);

// Add configuration options.
builder.Services.Configure<Configuration>(builder.Configuration.GetSection(Configuration.AppSettings));

// Add services to the container.
builder.Services.AddTransient<IPortfolioService, PortfolioService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IJob, SyncPortfolioJob>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
});
builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
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

await app.AddBackgroudJobs();

await app.RunAsync();
