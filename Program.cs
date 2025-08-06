using EvoTicketing.Data;
using EvoTicketing.Services;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOpenTelemetry()
.ConfigureResource(resource => resource.AddService("TheTicketShop"))
.WithMetrics(metrics =>
{
    metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation();
    metrics.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:18889");
    });
})
.WithTracing(tracing =>
{
    tracing
        .AddAspNetCoreInstrumentation()
        .AddGrpcClientInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation();
    tracing.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:18889");
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EvoTicketingDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("EvoTicketing"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddGrpc();
builder.Services.AddScoped<TicketService>();

var app = builder.Build();

app.MapGrpcService<TicketService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
