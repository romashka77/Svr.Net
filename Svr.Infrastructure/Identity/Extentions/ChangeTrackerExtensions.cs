﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Svr.Infrastructure.Identity.Extentions
{
    public static class ChangeTrackerExtensions
    {
        public static void ApplyAuditInformation(this ChangeTracker changeTracker)
        {
            foreach (var entry in changeTracker.Entries())
            {
                if (!(entry.Entity is ApplicationUser baseEntity)) continue;
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
