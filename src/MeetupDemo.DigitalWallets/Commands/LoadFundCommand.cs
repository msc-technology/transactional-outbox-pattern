using MeetupDemo.Shared;

namespace MeetupDemo.DigitalWallets.Commands;


public class LoadFundCommand : ICommand
{
    public int CustomerId { get; }
    public string CustomerName { get; }
    public string Currency { get; }
    public decimal Amount { get; }

    public LoadFundCommand(int customerId, string customerName, string currency, decimal amount)
    {
        CustomerId = customerId;
        CustomerName = string.IsNullOrEmpty(customerName) ? "Anonymous customer" : customerName;
        Currency = string.IsNullOrEmpty(currency) ? "USD" : currency;
        Amount = amount;
    }
}
