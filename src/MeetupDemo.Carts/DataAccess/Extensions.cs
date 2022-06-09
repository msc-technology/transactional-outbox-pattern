using Microsoft.EntityFrameworkCore;

namespace MeetupDemo.Carts.DataAccess;

public static class Extensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<CartDbContext>(x =>
            x.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = Cart; Trusted_Connection = True; MultipleActiveResultSets = true"));

        return services;
    }
}
