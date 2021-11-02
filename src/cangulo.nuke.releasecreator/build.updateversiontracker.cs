using cangulo.nuke.releasecreator.Constants;
using cangulo.nuke.releasecreator.Models;
using cangulo.nuke.releasecreator.Repository;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using System.IO;
using System.Text.Json;

internal partial class Build : NukeBuild
{
    private Target UpdateVersionTrackerFile => _ => _
        .DependsOn(SetNextReleaseNumber)
        .Before(PushFilesUpdatedWithNewVersion)
        .Executes(() =>
        {
            ParseSettings();
            var serviceProvider = SetupServices();

            var resultBagRepository = serviceProvider.GetRequiredService<IResultBagRepository>();
            var nextVersion = GetNextVersion(resultBagRepository);

            var versionTrackerFilePath = ReleaseSettings.VersionTrackerFilePath;
            SetVersion(nextVersion, versionTrackerFilePath);
            Logger.Success($"updated current version ({nextVersion}) in {versionTrackerFilePath}");

            EnqueueFileToBePushed(resultBagRepository, versionTrackerFilePath);
        });

    private static void SetVersion(string nextVersion, string filePath)
    {
        var fileContent = File.ReadAllText(filePath);
        var currentVersionFileModel = JsonSerializer.Deserialize<CurrentVersionFileModel>(fileContent, SerializerContants.DESERIALIZER_OPTIONS);

        currentVersionFileModel.CurrentVersion = nextVersion;

        var newCurrentVersionJson = JsonSerializer.Serialize(currentVersionFileModel, SerializerContants.SERIALIZER_OPTIONS);

        using StreamWriter fileWriter = new(filePath, append: false);
        fileWriter.Write(newCurrentVersionJson);
    }
}