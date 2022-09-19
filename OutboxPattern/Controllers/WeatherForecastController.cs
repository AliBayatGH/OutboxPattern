using DotNetCore.CAP;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OutboxPattern.Application.Commands;

namespace OutboxPattern.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WeatherForecastController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderConnand commend)
        {
            await _mediator.Send(commend);

            return Ok();
        }

        [HttpGet]
        public IActionResult SendMessage([FromServices] ICapPublisher capBus)
        {
            capBus.Publish("test.show.time", DateTime.Now);

            return Ok();
        }

        [NonAction]
        [CapSubscribe("test.show.time")]
        public void ReceiveMessage(DateTime time)
        {
            Console.WriteLine("message time is:" + time);
        }
    }
}