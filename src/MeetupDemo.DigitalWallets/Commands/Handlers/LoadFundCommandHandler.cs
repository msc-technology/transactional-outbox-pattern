using System.Text.Json;
using MeetupDemo.DigitalWallets.DataAccess;
using MeetupDemo.DigitalWallets.DataAccess.Models;
using MeetupDemo.DigitalWallets.Messages;
using MeetupDemo.Shared;
using MeetupDemo.Shared.Accessors;
using MeetupDemo.Shared.Options;
using MeetupDemo.Shared.Publishers;
using Microsoft.EntityFrameworkCore;

namespace MeetupDemo.DigitalWallets.Commands.Handlers;

internal sealed class LoadFundCommandHandler : ICommandHandler<LoadFundCommand>
{
    private readonly DigitalWalletDbContext _dbContext;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IMessageIdAccessor _messageIdAccessor;
    private readonly IMessageOutbox _outbox;
    private readonly OutboxOptions _outboxOptions;
    public LoadFundCommandHandler(
        DigitalWalletDbContext dbContext,
        IMessagePublisher messagePublisher,
        IMessageIdAccessor messageIdAccessor,
        IMessageOutbox outbox,
        OutboxOptions outboxOptions)
    {
        _dbContext = dbContext;
        _messagePublisher = messagePublisher;
        _messageIdAccessor = messageIdAccessor;
        _outbox = outbox;
        _outboxOptions = outboxOptions;
    }

    public async Task HandleAsync(LoadFundCommand command, CancellationToken cancellationToken = default)
    {
        var fund = await _dbContext.Funds.SingleOrDefaultAsync(x => x.CustomerId == command.CustomerId && x.Currency == command.Currency);

        if (fund is null)
        {
            fund = new CustomerFund
            {
                CustomerId = command.CustomerId,
                CurrentFund = command.Amount,
                Currency = command.Currency,
                CustomerName = command.CustomerName
            };

            _dbContext.Funds.Add(fund);
        }
        else
        {
            fund.CurrentFund += command.Amount;
            _dbContext.Funds.Update(fund);
        }

        var exchange = "DigitalWallet.Funds";
        var currency = fund.Currency.ToUpper();
        var message = new CustomerFundMessage(fund.CustomerId, currency, fund.CurrentFund);
        var routingKey = $"{currency}.{fund.CustomerName}";

        if (_outbox.Enabled)
        {
            await _outbox.SendAsync(exchange, routingKey, message);
            return;
        }

        if (_outboxOptions.ShouldThrowSqlException)
        {
            throw new Exception("SQL Error: Updating CustomerFunds table failed!");
        }

        await _dbContext.SaveChangesAsync();

        var messageJson = JsonSerializer.Serialize(message);
        await _messagePublisher.PublishAsync(exchange, routingKey, messageJson, _messageIdAccessor.GetMessageId());
    }
}
