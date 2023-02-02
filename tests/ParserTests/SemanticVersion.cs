using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Parser.SemanticVersion;

namespace ParserTests;

public class SemanticVersion
{
    [Fact(DisplayName = "Can parse simple semver")]
    public void Can_parse_simple()
    {
        var parseResult =
            Parser.SemanticVersion.SemanticVersion.TryParse("1.2.3-foo+bar",
                out var parsedSemver, out var errors);
        
        errors.Should().BeNull("valid semvers should return ");

        parsedSemver.Major.Should().Be(1);
        parsedSemver.Minor.Should().Be(2);
        parsedSemver.Patch.Should().Be(3);

        parsedSemver.PreRelease.Should().Be("foo");
        parsedSemver.Build.Should().Be("bar");
    }

    [Theory(DisplayName = "Can parse tagged semver")]
    [InlineData("1.2.3-alpha3+bar")]
    [InlineData("1.2.3-alpha.3+bar")]
    [InlineData("1.2.3-alpha-3+bar")]
    public void Can_parse_tagged(string msg)
    {
        var parseResult =
            Parser.SemanticVersion.SemanticVersion.TryParse(msg,
                out var parsedSemver, out var errors);
        
        errors.Should().BeNull("valid semvers should return ");

        parsedSemver.Major.Should().Be(1);
        parsedSemver.Minor.Should().Be(2);
        parsedSemver.Patch.Should().Be(3);

        parsedSemver.Build.Should().Be("bar");
        parsedSemver.PreReleaseVersion.Tag.Should().Be(SemanticTag.Alpha);
        parsedSemver.PreReleaseVersion.Version.Should().Be(3);
    }

}