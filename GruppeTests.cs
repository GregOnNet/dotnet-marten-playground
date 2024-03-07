using FluentAssertions;

namespace Perosnaldisposition;

public class GruppeTests
{
    [Fact]
    public void Wenn_eine_Gruppe_ohne_Namen_angelegt_wird_schlägt_dies_fehl()
    {
        var name = "";
        var result = Gruppe.Create(name);

        result.IsFailure.Should().BeTrue();
    }
}