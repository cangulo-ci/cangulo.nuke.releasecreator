using cangulo.nuke.releasecreator.Models;
using cangulo.nuke.releasecreator.Repository;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using System.IO;
using System.Text.RegularExpressions;

internal partial class Build : NukeBuild
{
    private Target UpdateVersionInCSProj => _ => _
        .DependsOn(SetNextReleaseNumber)
        .Before(PushFilesUpdatedWithNewVersion)
        .OnlyWhenDynamic(() => UpdateCsProjSettingsAreDefined())
        .Executes(() =>
        {
            ParseSettings();
            var serviceProvider = SetupServices();

            var resultBagRepository = serviceProvider.GetRequiredService<IResultBagRepository>();
            var nextVersion = GetNextVersion(resultBagRepository);

            var settings = ReleaseSettings.UpdateVersionInCSProjSettings;
            var projectPath = settings.ProjectPath;

            using var reader = new StreamReader(projectPath);
            var csprojContent = reader.ReadToEnd();
            reader.Close();

            var pattern = @"<Version>(.*)<\/Version>";

            var versionTagDefined = Regex.IsMatch(csprojContent, pattern, RegexOptions.IgnoreCase);
            ControlFlow.Assert(versionTagDefined, $"Please define <Version>[CURRENT_VERSION]</Version> in the csproj {projectPath}");

            var newVersionText = string.Empty;

            if (string.IsNullOrEmpty(settings.PreReleaseVersionSuffix))
                newVersionText = $"<Version>{nextVersion}</Version>";
            else
            {
                var versionSuffix = settings.PreReleaseVersionSuffix;
                newVersionText = $"<Version>{nextVersion}-{versionSuffix}</Version>";
            }

            var newContent = Regex.Replace(csprojContent, pattern, newVersionText);

            using var writer = new StreamWriter(projectPath);
            writer.Write(newContent);
            writer.Close();

            Logger.Success($"updated version {newVersionText} in the csproj file {projectPath}");

            EnqueueFileToBePushed(resultBagRepository, projectPath);
        });

    private bool UpdateCsProjSettingsAreDefined()
    {
        ParseSettings();
        return ReleaseSettings.UpdateVersionInCSProjSettings is not null;
    }
}