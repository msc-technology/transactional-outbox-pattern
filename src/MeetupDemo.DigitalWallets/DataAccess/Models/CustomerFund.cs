using Microsoft.EntityFrameworkCore;

namespace MeetupDemo.DigitalWallets.DataAccess.Models;

[Index(nameof(CustomerId), nameof(Currency), IsUnique = true)]
public class CustomerFund
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string Currency { get; set; }
    public decimal CurrentFund { get; set; }
}
