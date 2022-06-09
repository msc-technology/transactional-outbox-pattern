using Microsoft.EntityFrameworkCore;

namespace MeetupDemo.Carts.DataAccess.Models;

[Index(nameof(CustomerId), nameof(Currency), IsUnique = true)]
public class CustomerAvailabilityFund
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Currency { get; set; }
    public decimal AvailabilityFund { get; set; }
}
