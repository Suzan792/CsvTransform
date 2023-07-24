using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

[assembly: CLSCompliant(false)]

namespace CsvTimer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var path = args[0];
            IList<Dynamics> rows;
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ",";
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.IgnoreBlankLines = true;
                csv.Configuration.MissingFieldFound = null;

                var togglRows = csv
                    .GetRecords<Toggl>()
                    .ToList();

                rows = togglRows
                    .Select(t => new Dynamics
                    {
                        Type = GetType(t),
                        Project = t.Client,
                        ProjectTask = t.Project,
                        HourType = t.Task,
                        ExternalComments = t.Description,
                        Date = $"{DateTime.ParseExact(t.StartDate, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo).Date:dd-MM-yyyy}",
                        Duration = $"{t.Duration.TotalHours:0.00}",
                    })
                    .ToList();

                var totalTime = togglRows.Aggregate(TimeSpan.Zero, (sum, next) => sum + next.Duration);
                Console.WriteLine($"Toggl: {togglRows.Count} rows. {(int)totalTime.TotalHours}h {totalTime.Minutes}m");

                var jiraTime = togglRows
                    .Where(t => !t.Project.StartsWith("Source", StringComparison.OrdinalIgnoreCase))
                    .Aggregate(TimeSpan.Zero, (sum, next) => sum + next.Duration);
                Console.WriteLine($"Dynamics:  {rows.Count} rows. {(int)jiraTime.TotalHours}h {jiraTime.Minutes}m");
            }

            using (var writer = new StreamWriter(Path.Combine(Path.GetDirectoryName(path)!, $"{Path.GetFileNameWithoutExtension(path)}_dynamics{Path.GetExtension(path)}")))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(rows);
            }
        }

        private static string GetType(Toggl t)
        {
            return t.Client switch
            {
                "Vacation" => "Vacation",
                "Absence" => "Absence",
                _ => "Work",
            };
        }
    }
}
