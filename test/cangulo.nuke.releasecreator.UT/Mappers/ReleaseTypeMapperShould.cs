using cangulo.common.testing.dataatributes;
using cangulo.nuke.releasecreator.Mappers;
using cangulo.nuke.releasecreator.Models;
using FluentAssertions;
using Xunit;

namespace cangulo.nuke.releasecreator.UT.Mappers
{
    public static class ConstantsReleaseTypeMapperTests
    {
        public static ConventionalCommitTypeVsReleaseType[] ValidSettings =
            new ConventionalCommitTypeVsReleaseType[]
            {
                new ConventionalCommitTypeVsReleaseType
                {
                    CommitType = "patch",
                    ReleaseType = ReleaseType.Patch
                },
                new ConventionalCommitTypeVsReleaseType
                {
                    CommitType = "fix",
                    ReleaseType = ReleaseType.Patch
                },
                new ConventionalCommitTypeVsReleaseType
                {
                    CommitType = "feat",
                    ReleaseType = ReleaseType.Minor
                },
                new ConventionalCommitTypeVsReleaseType
                {
                    CommitType = "major",
                    ReleaseType = ReleaseType.Major
                }
            };

        public static ConventionalCommitTypeVsReleaseType[] ValidSettingsWithUndefinedReleaseType =
            new ConventionalCommitTypeVsReleaseType[]
            {
                        new ConventionalCommitTypeVsReleaseType
                        {
                            CommitType = "docs"
                        },
                        new ConventionalCommitTypeVsReleaseType
                        {
                            CommitType = "patch",
                            ReleaseType = ReleaseType.Patch
                        }
            };
    }

    public class ReleaseTypeMapperShould
    {
        [Theory]
        [InlineAutoNSubstituteData(ReleaseType.Patch, new string[] { "fix", "patch" })]
        [InlineAutoNSubstituteData(ReleaseType.Minor, new string[] { "fix", "patch", "feat" })]
        [InlineAutoNSubstituteData(ReleaseType.Minor, new string[] { "fix", "patch", "patch", "feat" })]
        [InlineAutoNSubstituteData(ReleaseType.Major, new string[] { "fix", "patch", "feat", "major" })]
        public void MatchReleaseType_when_InputMatchSettings(ReleaseType expectedReleaseType, string[] inputCommitTypes, ReleaseTypeMapper sut)
        {
            // Arrange
            var releaseTypeAllowed = ConstantsReleaseTypeMapperTests.ValidSettings;
            // Act
            var result = sut.MapReleaseType(inputCommitTypes, releaseTypeAllowed);

            // Assert
            result.Should().Be(expectedReleaseType);
        }

        [Theory]
        [InlineAutoNSubstituteData(ReleaseType.Undefined, new string[] { "docs", "refactor" })]
        public void MatchUndefinedReleaseType_When_InputMatchSettings(ReleaseType expectedReleaseType, string[] inputCommitTypes, ReleaseTypeMapper sut)
        {
            // Arrange
            var releaseTypeAllowed = ConstantsReleaseTypeMapperTests.ValidSettingsWithUndefinedReleaseType;
            // Act
            var result = sut.MapReleaseType(inputCommitTypes, releaseTypeAllowed);

            // Assert
            result.Should().Be(expectedReleaseType);
        }
    }
}