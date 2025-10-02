using Prometheus;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers;

[ApiController]
[Route("strings")]
public class StringController : ControllerBase
{
    private static readonly Counter Hits = Metrics.CreateCounter("demo_controller_hits", "The number of hits in the controller", new string[1] { "method" });

    private static readonly Histogram Slept = Metrics.CreateHistogram("demo_sleep_duration_seconds", "The seconds the handler slept for");

    [HttpGet("static")]
    public IResult Static()
    {
        Hits.WithLabels(new string[1] { "Static" }).Inc();
        return Results.Ok("string method");
    }

    [HttpGet("sleep")]
    public async Task<IResult> Sleep()
    {
        Hits.WithLabels(new string[1] { "Sleep" }).Inc();

        Random rnd = new Random(DateTime.Now.Millisecond);
        int duration = rnd.Next(100, 750);

        using (Slept.NewTimer())
        {
            await Task.Delay(duration);
        }

        string message = String.Format("slept for {0}ms", duration.ToString());
        return Results.Ok(message);
    }

    [HttpPost("error")]
    public IResult Throw()
    {
        SomeService service = new SomeService();
        service.doSomething();
        return Results.Ok("string method");
    }
}
