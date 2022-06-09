using Microsoft.EntityFrameworkCore;

namespace MeetupDemo.DigitalWallets.DataAccess;

public static class Extensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<DigitalWalletDbContext>(x =>
            x.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = DigitalWallet; Trusted_Connection = True; MultipleActiveResultSets = true"));

        return services;
    }
}
