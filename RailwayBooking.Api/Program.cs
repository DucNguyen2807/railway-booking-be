using RailwayBooking.Api.Extensions;
using RailwayBooking.Api.Middleware;
using Serilog;

//
// ========================================
// SERILOG
// ========================================
//

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//
// ========================================
// HOST LOGGING
// ========================================
//

builder.Host.UseSerilog();

//
// ========================================
// SERVICES
// ========================================
//

//
// Controllers
//
builder.Services.AddControllers();

//
// Swagger
//
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerDocumentation();

//
// Application Services
//
builder.Services.AddApplicationServices(builder.Configuration);

//
// Authentication
//
builder.Services.AddJwtAuthentication(builder.Configuration);

//
// CORS
//
builder.Services.AddCorsPolicy();

var app = builder.Build();

//
// ========================================
// MIDDLEWARE PIPELINE
// ========================================
//

//
// CorrelationId FIRST
//
app.UseMiddleware<CorrelationIdMiddleware>();

//
// Request logging
//
app.UseSerilogRequestLogging();

//
// Global exception handling
//
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<JwtBlacklistMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();

app.UseCorsPolicy();

app.UseRouting();

//
// Authentication
//
app.UseAuthentication();

//
// Authorization
//
app.UseAuthorization();

//
// Controllers
//
app.MapControllers();

app.Run();