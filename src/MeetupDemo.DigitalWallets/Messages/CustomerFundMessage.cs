using MeetupDemo.Shared;

namespace MeetupDemo.DigitalWallets.Messages;

public record CustomerFundMessage(int CustomerId, string Currency, decimal CurrentFunds) : IMessage;