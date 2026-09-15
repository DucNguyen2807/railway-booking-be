using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Logging
{
    public interface IAuditService
    {
        Task LogAsync(
            string entityType,
            long entityId,
            string action,
            object? oldValue = null,
            object? newValue = null,
            Guid? requestId = null,
            long? userId = null);
    }
}
