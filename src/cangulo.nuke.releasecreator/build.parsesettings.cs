using cangulo.nuke.releasecreator.Constants;
using cangulo.nuke.releasecreator.Models;
using Nuke.Common;
using System;
using System.IO;
using System.Text.Json;

internal partial class Build : NukeBuild
{
    private void ParseSettings()
    {
        if (File.Exists(ReleaseSettingsPath))
        {
            var fileContent = File.ReadAllText(ReleaseSettingsPath);
            ReleaseSettings = JsonSerializer.Deserialize<ReleaseSettings>(fileContent, SerializerContants.DESERIALIZER_OPTIONS);

            Logger.Trace($"Request Mapped {JsonSerializer.Serialize(ReleaseSettings, SerializerContants.SERIALIZER_OPTIONS)}");
        }
        else
            throw new Exception("cicd.json not provided in the root directory");
    }
}