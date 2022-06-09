
using MeetupDemo.Carts.DataAccess;
using MeetupDemo.Carts.DataAccess.Models;
using MeetupDemo.Shared;
using Microsoft.EntityFrameworkCore;

namespace MeetupDemo.Carts.Messages.Handlers;

internal sealed class FundsMessageHandler : IMessageHandler<FundsMessage>
{
    private readonly CartDbContext _dbContext;

    public FundsMessageHandler(CartDbContext dbContext)
        => _dbContext = dbContext;

    public async Task HandleAsync(FundsMessage message, CancellationToken cancellationToken = default)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var availabilityFund = await _dbContext.AvailabilityFunds
            .SingleOrDefaultAsync(x => x.CustomerId == message.CustomerId && x.Currency == message.Currency);

        if (availabilityFund is null)
        {
            availabilityFund = new CustomerAvailabilityFund
            {
                CustomerId = message.CustomerId,
                Currency = message.Currency,
                AvailabilityFund = message.CurrentFunds
            };

            await _dbContext.AvailabilityFunds.AddAsync(availabilityFund);
        }
        else
        {
            availabilityFund.AvailabilityFund = message.CurrentFunds;
            _dbContext.AvailabilityFunds.Update(availabilityFund);
        }

        await _dbContext.SaveChangesAsync();
    }
}
