﻿using System;
 using System.Threading.Tasks;

 namespace Traderr.Polygon.Mining.Api.Core.Infrastructure.Jobs.Interfaces
{
    public interface IRecurringJob : IJob
    {
        string CronInterval { get; }
        TimeZoneInfo TimeZoneInfo { get; }
    }
}