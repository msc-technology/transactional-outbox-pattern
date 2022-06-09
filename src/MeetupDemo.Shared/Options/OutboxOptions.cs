namespace MeetupDemo.Shared.Options;

public class OutboxOptions
{
    public bool Enabled { get; set; }
    public bool ShouldThrowNetworkException { get; set; }
    public bool ShouldThrowSqlException { get; set; }
    public double IntervalMilliseconds { get; set; }
}
