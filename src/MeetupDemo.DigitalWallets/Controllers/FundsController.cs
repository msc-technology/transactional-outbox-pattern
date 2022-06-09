using MeetupDemo.DigitalWallets.Commands;
using MeetupDemo.Shared.Dispatchers;
using Microsoft.AspNetCore.Mvc;

namespace MeetupDemo.DigitalWallets.Controllers;

[Route("[controller]")]
[ApiController]
public class FundsController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public FundsController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPut]
    public async Task<ActionResult> LoadFund(LoadFundCommand command)
    {
        await _commandDispatcher.SendAsync(command);
        return Ok();
    }
}
