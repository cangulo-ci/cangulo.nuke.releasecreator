using cangulo.nuke.releasecreator.Constants;
using cangulo.nuke.releasecreator.Models;
using cangulo.nuke.releasecreator.Repository;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using System.Collections.Generic;

internal partial class Build : NukeBuild
{
    private Target PushFilesUpdatedWithNewVersion => _ => _
        .DependsOn(SetNextReleaseNumber)
        .After(
            UpdateChangelog,
            UpdateVersionTrackerFile
            )
        .Executes(() =>
        {
            ParseSettings();
            var serviceProvider = SetupServices();

            ControlFlow.NotNull(ReleaseSettings.GitPushReleaseFilesSettings);

            var resultBagRepository = serviceProvider.GetRequiredService<IResultBagRepository>();
            var nextVersion = GetNextVersion(resultBagRepository);

            Git($"config --global user.email \"{ReleaseSettings.GitPushReleaseFilesSettings.Email}\"");
            Git($"config --global user.name \"{ReleaseSettings.GitPushReleaseFilesSettings.Name}\"");

            var filesQueue = resultBagRepository.GetResultOrDefault(ResultBagConstants.RELEASE_FILES_TO_PUSH, new Queue<string>());
            ControlFlow.NotEmpty(filesQueue, "no files to push");

            foreach (var file in filesQueue)
                Git($"add {file}", logOutput: true);

            var foldersPath = ReleaseSettings.GitPushReleaseFilesSettings.FoldersPath;
            foreach (var folderPath in foldersPath)
                Git($"add {folderPath}/**", logOutput: true);

            var filesPath = ReleaseSettings.GitPushReleaseFilesSettings.FilesPath;
            foreach (var filePath in filesPath)
                Git($"add {filePath}", logOutput: true);

            Git($"commit -m \"RELEASE v{nextVersion} : updated version in files \"", logOutput: true);
            Git($"push", logOutput: true);
        });

    private static void EnqueueFileToBePushed(IResultBagRepository resultBagRepository, string path)
    {
        var queue = resultBagRepository.GetResultOrDefault(ResultBagConstants.RELEASE_FILES_TO_PUSH, new Queue<string>());
        queue.Enqueue(path);
        resultBagRepository.AddOrReplaceResult(ResultBagConstants.RELEASE_FILES_TO_PUSH, queue);
        Logger.Info($"enqueued {path} to be pushed");
    }
}