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
    
    [Fact]
    public void Wenn_der_Name_einer_Gruppe_korrigiert_wird_schlägt_dies_mit_einem_leeren_Namen_fehl()
    {
        var name = "Meine Gruppe";
        var gruppe = Gruppe.Create(name);

        var result = gruppe.Value.KorrigiereNamen(string.Empty);

        result.Should().Fail();
    }
}