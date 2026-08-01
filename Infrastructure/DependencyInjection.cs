using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services;
using Infrastructure.Context;
using Infrastructure.Context.Factories;
using Infrastructure.Context.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Generic repository
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            // Services (tables)
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IBoothService, BoothService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IDashBoardService, DashBoardService>();
            services.AddScoped<IPhotoServices, PhotoService>();
            services.AddScoped<IFrameService, FrameService>();
            services.AddScoped<ITopicService, TopicService>();
            
            return services;
        }
    }
}
