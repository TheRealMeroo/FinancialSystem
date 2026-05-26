using Accounting.Domain.Repositories;
using Accounting.Infrastructure.Persistance.Data;
using Accounting.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccountingDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IDepositRequestRepository, DepositRequestRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IJournalEntryRepository, JournalEntyRepository>();

        return services;
    }
}
