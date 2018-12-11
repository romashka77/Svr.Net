using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Svr.Core.Entities;

namespace Svr.Infrastructure.Data.Extentions
{
    public static class ChangeTrackerExtensions
    {
        public static void ApplyAuditInformation(this ChangeTracker changeTracker)
        {
            foreach (var entry in changeTracker.Entries())
            {
                if (!(entry.Entity is BaseEntity baseEntity)) continue;
                var now = DateTime.UtcNow;
                switch (entry.State)
                {
                    case EntityState.Modified:
                        baseEntity.UpdatedOnUtc = now;
                        break;
                    case EntityState.Added:
                        baseEntity.CreatedOnUtc = now;
                        baseEntity.UpdatedOnUtc = now;
                        break;
                    default:
                        break;
                }

            }
        }
    }
}
