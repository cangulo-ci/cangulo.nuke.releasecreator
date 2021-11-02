using cangulo.changelog.Extensions;
using cangulo.nuke.releasecreator.Helpers;
using cangulo.nuke.releasecreator.Mappers;
using cangulo.nuke.releasecreator.Models;
using cangulo.nuke.releasecreator.Parsers;
using cangulo.nuke.releasecreator.Repository;
using cangulo.nuke.releasecreator.Services;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;

internal partial class Build : NukeBuild
{
    private ServiceProvider SetupServices()
    {
        var services = new ServiceCollection();

        services
                .AddTransient<ICommitParser, CommitParser>()
                .AddTransient<IReleaseNumberParser, ReleaseNumberParser>()
                .AddTransient<IResultBagRepository, ResultBagRepository>(s => new ResultBagRepository(ResultBagFilePath))
                .AddTransient<IReleaseTypeMapper, ReleaseTypeMapper>()
                .AddTransient<INextReleaseNumberHelper, NextReleaseNumberHelper>();

        if (ReleaseSettings.ChangelogSettings is not null)
        {
            services
                .AddChangelogServices(ReleaseSettings.ChangelogSettings);
        }

        return services.BuildServiceProvider(true);
    }
}