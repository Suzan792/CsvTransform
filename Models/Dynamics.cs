using CsvHelper.Configuration.Attributes;

namespace CsvTimer
{
    public class Dynamics
    {
        [Name("Type")]
        public string Type { get; init; } = null!;

        [Name("Project")]
        public string Project { get; init; } = null!;

        [Name("Project Task")]
        public string ProjectTask { get; init; } = null!;

        [Name("Hour Type")]
        public string HourType { get; init; } = null!;

        [Name("External Comments")]
        public string ExternalComments { get; init; } = null!;

        [Name("Jira Ticket")]
        public string JiraTicket { get; init; } = null!;

        [Name("Date")]
        public string Date { get; init; } = null!;

        [Name("Duration")]
        public string Duration { get; init; } = null!;

        [Name("Role")]
        public string Role { get; init; } = null!;

        public override string ToString()
        {
            return $"{Type}, {Project}, {ProjectTask}, {HourType}, {JiraTicket}, {ExternalComments}, {Date}, {Duration}";
        }
    }
}
