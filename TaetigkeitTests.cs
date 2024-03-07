using FluentAssertions;

namespace Perosnaldisposition;

public class TaetigkeitTests
{
    [Fact]
    public void Wenn_eine_Tätigkeit_ohne_Namen_angelegt_wird_schlägt_dies_fehl()
    {
        var name = "";
        var result = Taetigkeit.Create(name);

        result.IsFailure.Should().BeTrue();
    }
}