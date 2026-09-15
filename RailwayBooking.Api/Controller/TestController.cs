using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Infrastructure.Entities.Railway;
using RailwayBooking.Infrastructure.Persistence;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly BookingDbContext _db;

    public TestController(BookingDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var trains = await _db.Trains.ToListAsync();

        return Ok(trains);
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var train = new Train
        {
            Code = Guid.NewGuid().ToString().Substring(0, 6),
            Name = "Test Train",
            Status = 1,
            CreatedAt = DateTime.UtcNow
        };

        _db.Trains.Add(train);
        await _db.SaveChangesAsync();

        return Ok(train);
    }
}