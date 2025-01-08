﻿using Application.Interfaces;
using Application.Services;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);

            services.AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic));
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IGymApplication, GymApplication>();
            services.AddScoped<IAthleteApplication, AthleteApplication>();
            services.AddScoped<IMembershipApplication, MembershipApplication>();
            services.AddScoped<IDiscountApplication, DiscountApplication>();
            services.AddScoped<IEmailServiceApplication, EmailServiceApplication>();
            services.AddScoped<IJwtHandler, JwtHandler>();
            services.AddScoped<IDashboardApplication, DashboardApplication>();
            services.AddScoped<IInventoryProductsApplication, InventoryProductsApplication>();
            services.AddScoped<IOrdersPaymentsApplication, OrdersPaymentsApplication>();
            services.AddScoped<ICryptographyApplication, CryptographyApplication>();
            services.AddScoped<INotificationApplication, NotificationApplication>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();

            return services;
        }
    }
}
