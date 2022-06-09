using MeetupDemo.Shared;

namespace MeetupDemo.Carts.Messages;

public record FundsMessage(int CustomerId, string Currency, decimal CurrentFunds) : IMessage;