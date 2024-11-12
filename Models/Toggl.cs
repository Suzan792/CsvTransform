using System;
using CsvHelper.Configuration.Attributes;

namespace CsvTimer
{
    public class Toggl
    {
        public string User { get; init; } = null!;

        public string Email { get; init; } = null!;

        public string Client { get; init; } = null!;

        public string Project { get; init; } = null!;

        public string Task { get; init; } = null!;

        public string Description { get; init; } = null!;

        public string Billable { get; init; } = null!;

        [Name("Start date")]
        public string StartDate { get; init; } = null!;

        [Name("Start time")]
        public string StartTime { get; init; } = null!;

        [Name("End date")]
        public string EndDate { get; init; } = null!;

        [Name("End time")]
        public string EndTime { get; init; } = null!;

        public TimeSpan Duration { get; init; }

        public string Tags { get; init; } = null!;
    }
}
