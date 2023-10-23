using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CsvHelper;
using Microsoft.Extensions.Configuration;

[assembly: CLSCompliant(false)]

namespace CsvTimer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            if (args.Length != 1)
            {
                throw new ArgumentException("expected 1 arg: path");
            }

            var path = args[0];

            var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Personal.json", optional: true)
                .Build();

            var role = config.GetSection("Role").Value;
            if (string.IsNullOrWhiteSpace(role) || string.Equals(role, "TODO", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Role (appsettings) is required");
            }

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
                        Project = Project(t),
                        ProjectTask = ProjectTask(t),
                        HourType = HourType(t),
                        ExternalComments = ExternalComments(t),
                        Date = $"{DateTime.ParseExact(t.StartDate, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo).Date:dd-MM-yyyy}",
                        Duration = Duration(t),
                        Role = role,
                    })
                    .ToList();

                var totalTime = togglRows.Aggregate(TimeSpan.Zero, (sum, next) => sum + next.Duration);
                Console.WriteLine($"Toggl: {togglRows.Count} rows. {(int)totalTime.TotalHours}h {totalTime.Minutes}m");

                var dynamicsTime = rows
                    .Aggregate(TimeSpan.Zero, (sum, next) => sum + TimeSpan.FromHours(double.Parse(next.Duration, CultureInfo.InvariantCulture)));
                Console.WriteLine($"Dynamics:  {rows.Count} rows. {(int)dynamicsTime.TotalHours}h {dynamicsTime.Minutes}m");
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

        private static string Project(Toggl t)
        {
            return t.Client switch
            {
                "Vacation" or "Absence" => string.Empty,
                _ => Regex.Replace(t.Client, @"\s+-\s+P\d\d\d+$", string.Empty)
            };
        }

        private static string ProjectTask(Toggl t)
        {
            return t.Client switch
            {
                "Vacation" => string.Empty,
                "Absence" => string.Empty,
                _ => t.Project,
            };
        }

        private static string HourType(Toggl t)
        {
            return t.Client switch
            {
                "Vacation" => "Vacation",
                "Absence" => "Absence",
                _ => t.Task switch
                {
                    null or "" => t.Tags,
                    _ => t.Task,
                },
            };
        }

        private static string ExternalComments(Toggl t)
        {
            if (!string.IsNullOrWhiteSpace(t.Description))
            {
                return t.Description;
            }

            return t.Client switch
            {
                "Vacation" => "Vacation",
                "Absence" => "Absence",
                _ => t.Description,
            };
        }

        private static string Duration(Toggl t)
        {
            var hours = t.Duration.TotalHours;
            var correctedHours = hours / 1.665;

            return correctedHours.ToString("F", CultureInfo.InvariantCulture);
        }
    }
}
