using System.Text.Json;
using RailwayBooking.Infrastructure.Entities.Infrastructure;
using RailwayBooking.Infrastructure.Logging;
using RailwayBooking.Infrastructure.Persistence;

public class AuditService : IAuditService
{
    private readonly BookingDbContext _db;

    public AuditService(BookingDbContext db)
    {
        _db = db;
    }

    public async Task LogAsync(
        string entityType,
        long entityId,
        string action,
        object? oldValue = null,
        object? newValue = null,
        Guid? requestId = null,
        long? userId = null)
    {
        var log = new AuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            OldValue = oldValue == null
                ? null
                : JsonSerializer.Serialize(oldValue),

            NewValue = newValue == null
                ? null
                : JsonSerializer.Serialize(newValue),

            RequestId = requestId,
            UserId = userId,

            CreatedAt = DateTime.UtcNow
        };

        _db.AuditLogs.Add(log);

        await _db.SaveChangesAsync();
    }
}