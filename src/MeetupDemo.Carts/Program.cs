using MeetupDemo.Carts.DataAccess;
using MeetupDemo.Carts.Services;
using MeetupDemo.Shared;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDataAccess();
builder.Services.AddMessaging();

builder.Services.AddHostedService<MessagingBackgroundService>();


var app = builder.Build();

app.MapGet("/", () => "Carts Service!");

app.Run();