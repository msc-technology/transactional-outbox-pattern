
using System.Text.Json;
using MeetupDemo.DigitalWallets.DataAccess;
using MeetupDemo.DigitalWallets.Messages;
using MeetupDemo.Shared;
using MeetupDemo.Shared.Publishers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDataAccess();
builder.Services.AddMessaging();
builder.Services.AddCommanding();
builder.Services.AddMessageOutbox<DigitalWalletDbContext>();

var app = builder.Build();

app.MapGet("/", () => "Digital Wallet Service!");
app.MapGet("/message/send/EUR/{customerName}", async (IMessagePublisher messagePublisher, string customerName) =>
{
    var message = new CustomerFundMessage(123, "EUR", 10.00m);
    var messageJson = JsonSerializer.Serialize(message);
    var messageId = Guid.NewGuid().ToString("N");
    await messagePublisher.PublishAsync("DigitalWallet.Funds", $"EUR.{customerName}", messageJson, messageId);
});

app.MapControllers();

app.Run();
