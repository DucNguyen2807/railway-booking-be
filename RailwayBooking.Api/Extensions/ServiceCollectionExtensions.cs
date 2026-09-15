using MediatR;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Application.Features.UserFeat.Auth.SignIn;
using RailwayBooking.Application.Interfaces;
using RailwayBooking.Application.Mappings;
using RailwayBooking.Application.Services;
using RailwayBooking.Application.Services.Impl;
using RailwayBooking.Api.Services;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Logging;
using RailwayBooking.Infrastructure.Persistence;
using RailwayBooking.Infrastructure.Repositories;

namespace RailwayBooking.Api.Extensions
{

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //
            // DbContext
            //
            services.AddDbContext<BookingDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            "DefaultConnection"),

                        npgsql =>
                        {
                            npgsql.EnableRetryOnFailure(
                                maxRetryCount: 5,

                                maxRetryDelay:
                                    TimeSpan.FromSeconds(10),

                                errorCodesToAdd:
                                    null);
                        });

                    //
                    // DEV ONLY
                    //
                    if (Environment
                        .GetEnvironmentVariable(
                            "ASPNETCORE_ENVIRONMENT")
                        == "Development")
                    {
                        options.EnableDetailedErrors();

                        options.EnableSensitiveDataLogging();

                        options.LogTo(
                            Console.WriteLine,
                            LogLevel.Information);
                    }
                });

            //
            // Http
            //
            services.AddHttpContextAccessor();

            services.AddHttpClient();

            //
            // MediatR
            //
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(
                    typeof(SignInCommandHandler)
                        .Assembly);
            });

            services.AddAutoMapper(typeof(UserProfile).Assembly);
            //
            // Cache
            //
            services.AddDistributedMemoryCache();

            //
            // Services
            //
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IUtilityService, UtilityService>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IBookingOrderCancellationService, BookingOrderCancellationService>();

            services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddScoped<IAuditService, AuditService>();

            services.AddHostedService<BookingPaymentTimeoutWorker>();

            return services;
        }
    }
}