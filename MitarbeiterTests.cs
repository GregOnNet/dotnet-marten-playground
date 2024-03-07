using FluentAssertions;

namespace Perosnaldisposition;

public class MitarbeiterTests
{
    [Fact]
    public void Wenn_ein_Mitarbeiter_ohne_Vornamen_angelegt_wird_schlägt_dies_fehl()
    {
        var vorname = "";
        var nachname = "";
        var gruppe = Gruppe.Create("Frühschicht");

        
        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value);

        mitarbeiter.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_ohne_Nachnamen_angelegt_wird_schlägt_dies_fehl()
    {
        var vorname = "Elon";
        var nachname = "";
        var gruppe = Gruppe.Create("Frühschicht");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value);

        mitarbeiter.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Wenn_ein_Mitarbeiter_ohne_Gruppe_angelegt_wird_schlägt_dies_fehl()
    {
        var vorname = "Elon";
        var nachname = "";

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, null);

        mitarbeiter.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_mit_allen_nötigen_Informationen_angelegt_wird_funktioniert_es()
    {
        var vorname = "Elon";
        var nachname = "Musk";
        var gruppe = Gruppe.Create("Frühschicht");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value);

        mitarbeiter.Value.Vorname.Should().Be(vorname);
        mitarbeiter.Value.Nachname.Should().Be(nachname);
    }
    
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_angelegt_wurde_ist_er_für_keine_Tätigkeit_qualifiziert()
    {
        var vorname = "Elon";
        var nachname = "Musk";
        var gruppe = Gruppe.Create("Frühschicht");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value).Value;

        mitarbeiter.QualifizierteTaetigkeiten.Should().BeEmpty();
    }
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_über_keine_Qualifizierte_Taetigkeit_verfügt_kann_keine_Taetigkeit_zugewiesen_werden()
    {
        var vorname = "Elon";
        var nachname = "Musk";
        var gruppe = Gruppe.Create("Frühschicht");
        var taetigkeit = Taetigkeit.Create("Einlagern");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value).Value;

        var result = mitarbeiter.WeiseTaetigkeitZu(taetigkeit.Value);

        result.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_für_eine_Tätigkeit_qualifiziert_wird_kann_diese_Tätigkeit_zugewiesen_werden()
    {
        var vorname = "Elon";
        var nachname = "Musk";
        
        var gruppe = Gruppe.Create("Frühschicht");
        var taetigkeit = Taetigkeit.Create("Einlagern");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value).Value;

        mitarbeiter.QualifiziereFuerTaetigkeit(taetigkeit.Value);
        
        var result = mitarbeiter.WeiseTaetigkeitZu(taetigkeit.Value);

        result.IsSuccess.Should().BeTrue();

        mitarbeiter.ZugewieseneTaetigkeiten.Should().HaveCount(1);
    }
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_für_eine_Tätigkeit_qualifiziert_ist_kann_die_gleiche_Taetigkeit_nicht_noch_einmal_qualifiziert_werden()
    {
        var vorname = "Elon";
        var nachname = "Musk";
        
        var gruppe = Gruppe.Create("Frühschicht");
        var taetigkeit = Taetigkeit.Create("Einlagern");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value).Value;

        var qualifizierung = mitarbeiter.QualifiziereFuerTaetigkeit(taetigkeit.Value);
        var doppelteQualifizierung = mitarbeiter.QualifiziereFuerTaetigkeit(taetigkeit.Value);

        qualifizierung.IsSuccess.Should().BeTrue();
        doppelteQualifizierung.IsFailure.Should().BeTrue();
    }
}