namespace Husa.Uploader.Desktop.Factories
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using Dapplo.Jira;
    using Dapplo.Jira.Entities;
    using Husa.Uploader.Core.Models;

    public class JiraService : IJiraService
    {
        private IJiraClient jiraClient;

        public void InitializeBasicAuth(string baseUrl, string email, string apiToken)
        {
            this.jiraClient = JiraClient.Create(new Uri(baseUrl));
            this.jiraClient.SetBasicAuthentication(email, apiToken);
        }

        public async Task<string> CreateBugAsync(string summary, string description, string projectKey, List<UploaderError> uploaderErrors)
        {
            try
            {
                var (logContent, fileName) = this.GenerateLogInMemory(summary, description, uploaderErrors);
                var issue = new IssueWithFields<IssueFields>
                {
                    Fields = new IssueFields
                    {
                        Project = new Project { Key = projectKey },
                        IssueType = new IssueType { Name = "Bug" },
                        Summary = summary,
                        Description = description,
                    },
                };

                var createdIssue = await this.jiraClient.Issue.CreateAsync<IssueFields>(issue);
                var issueKey = createdIssue.Key;

                await this.AttachInMemoryFile(issueKey, logContent, fileName);

                return issueKey;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating bug: {ex}");
                throw;
            }
        }

        public string FormatErrorsForLog(string summary, string description, IEnumerable<UploaderError> errors)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== APPLICATION ERROR REPORT ===");
            sb.AppendLine($"Bug Report: {summary}");
            sb.AppendLine($"Report Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"User: {Environment.UserName}");
            sb.AppendLine($"Machine: {Environment.MachineName}");
            sb.AppendLine($"OS: {Environment.OSVersion}");
            sb.AppendLine($"App Version: {this.GetAppVersion()}");
            sb.AppendLine("\nDescription:");
            sb.AppendLine(description);
            sb.AppendLine($"Error Count: {errors.Count()}");
            sb.AppendLine();

            sb.AppendLine("===== ERROR DETAILS =====");
            sb.AppendLine("| Field ID       | Field Label      | Section           | User Message                     |");
            sb.AppendLine("|----------------|------------------|-------------------|----------------------------------|");

            foreach (var error in errors)
            {
                sb.AppendLine($"| {PadRight(error.FieldId, 15)} " +
                              $"| {PadRight(error.FieldLabel, 16)} " +
                              $"| {PadRight(error.FieldSection, 17)} " +
                              $"| {PadRight(error.FriendlyErrorMessage, 32)} |");
            }

            sb.AppendLine();

            sb.AppendLine("===== TECHNICAL DETAILS =====");
            foreach (var error in errors)
            {
                sb.AppendLine($"--- Error: {error.FieldId} ({error.FieldLabel}) ---");
                sb.AppendLine($"Field Info: {error.FieldInfo}");
                sb.AppendLine("Error Message:");
                sb.AppendLine(error.ErrorMessage);
                sb.AppendLine("----------------------------------------");
                sb.AppendLine();
            }

            sb.AppendLine("===== SYSTEM INFORMATION =====");
            sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($".NET Version: {Environment.Version}");
            sb.AppendLine($"Processor Count: {Environment.ProcessorCount}");
            sb.AppendLine($"Working Set: {this.FormatBytes(Environment.WorkingSet)}");
            sb.AppendLine($"System Page Size: {this.FormatBytes(Environment.SystemPageSize)}");
            sb.AppendLine($"Current Directory: {Environment.CurrentDirectory}");
            sb.AppendLine($"User Domain: {Environment.UserDomainName}");
            sb.AppendLine($"Culture: {CultureInfo.CurrentCulture.Name}");
            sb.AppendLine($"UTC Offset: {TimeZoneInfo.Local.GetUtcOffset(DateTime.Now)}");
            sb.AppendLine($"Is 64-bit OS: {Environment.Is64BitOperatingSystem}");
            sb.AppendLine($"Is 64-bit Process: {Environment.Is64BitProcess}");

            return sb.ToString();
        }

        private static string PadRight(string input, int length)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new string(' ', length);
            }

            return input.Length > length
                ? string.Concat(input.AsSpan(0, length - 3), "...")
                : input.PadRight(length);
        }

        private (byte[] Content, string FileName) GenerateLogInMemory(string summary, string description, List<UploaderError> uploadErrors)
        {
            var logContent = this.FormatErrorsForLog(summary, description, uploadErrors);

            var cleanSummary = new string(summary
                .Where(c => !Path.GetInvalidFileNameChars().Contains(c))
                .ToArray());

            var fileName = $"buglog_{DateTime.Now:yyyyMMdd_HHmmss}_{cleanSummary.Substring(0, Math.Min(20, cleanSummary.Length))}.txt";

            return (Encoding.UTF8.GetBytes(logContent.ToString()), fileName);
        }

        private async Task AttachInMemoryFile(string issueKey, byte[] fileContent, string fileName)
        {
            using (var memoryStream = new MemoryStream(fileContent))
            {
                await this.jiraClient.Attachment.AttachAsync(issueKey, memoryStream, fileName);
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            double size = bytes;

            while (size >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                size /= 1024;
                suffixIndex++;
            }

            return $"{size:0.##} {suffixes[suffixIndex]}";
        }

        private string GetAppVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return versionInfo.FileVersion ?? assembly.GetName().Version?.ToString() ?? "Unknown";
        }
    }
}
