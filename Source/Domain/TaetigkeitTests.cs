using FluentAssertions;

namespace Perosnaldisposition;

public class TaetigkeitTests
{
    [Fact]
    public void Wenn_eine_Tätigkeit_ohne_Namen_angelegt_wird_schlägt_dies_fehl()
    {
        var bezeichnung = "";
        var result = Taetigkeit.Create(bezeichnung);

        result.Should().Fail();
    }
    
    [Fact]
    public void Wenn_die_Bezeichnung_einer_Tätigkeit_korrigiert_wird_wird_die_neue_Bezeichnung_gesetzt()
    {
        var bezeichnung = "Einlgern";
        var neueBezeichnung = "Einlagern";
        
        var taetigkeit = Taetigkeit.Create(bezeichnung).Value;

        var korrigierteTaetigkeit = taetigkeit.KorrigiereBezeichnung(neueBezeichnung).Value;
        
        korrigierteTaetigkeit.Bezeichnung.Should().Be(neueBezeichnung);
    }
}