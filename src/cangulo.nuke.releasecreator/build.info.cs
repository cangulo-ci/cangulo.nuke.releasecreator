using cangulo.nuke.releasecreator.Constants;
using Nuke.Common;
using System.Linq;
using System.Text.Json;


internal partial class Build : NukeBuild
{
#pragma warning disable IDE0051 // Remove unused private members
    private Target PrintPipelineInfo => _ => _
#pragma warning restore IDE0051 // Remove unused private members
        .Executes(() =>
        {
            Logger.Success($"GitRepo");
            Logger.Info(JsonSerializer.Serialize(GitRepository, SerializerContants.SERIALIZER_OPTIONS));

            Logger.Success($"GitHubAction");
            Logger.Info(JsonSerializer.Serialize(GitHubActions, SerializerContants.SERIALIZER_OPTIONS));

            Logger.Success($"Environment Variables:");
            EnvironmentInfo.Variables
                .ToList()
                .ForEach(x => Logger.Info($"{x.Key} : {x.Value}"));
        });
}
