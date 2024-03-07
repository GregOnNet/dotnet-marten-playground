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

        mitarbeiter.Should().Fail();
    }
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_ohne_Nachnamen_angelegt_wird_schlägt_dies_fehl()
    {
        var vorname = "Alan";
        var nachname = "";
        var gruppe = Gruppe.Create("Frühschicht");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value);

        mitarbeiter.Should().Fail();
    }

    [Fact]
    public void Wenn_ein_Mitarbeiter_ohne_Gruppe_angelegt_wird_schlägt_dies_fehl()
    {
        var vorname = "Alan";
        var nachname = "";

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, null);

        mitarbeiter.Should().Fail();
    }
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_mit_allen_nötigen_Informationen_angelegt_wird_funktioniert_es()
    {
        var vorname = "Alan";
        var nachname = "Turing";
        var gruppe = Gruppe.Create("Frühschicht");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value);

        mitarbeiter.Should().Succeed();
        mitarbeiter.Value.Vorname.Should().Be(vorname);
        mitarbeiter.Value.Nachname.Should().Be(nachname);
    }
    
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_angelegt_wurde_ist_er_für_keine_Tätigkeit_qualifiziert()
    {
        var vorname = "Alan";
        var nachname = "Turing";
        var gruppe = Gruppe.Create("Frühschicht");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value);

        mitarbeiter.Value.QualifizierteTaetigkeiten.Should().BeEmpty();
    }
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_über_keine_Qualifizierte_Taetigkeit_verfügt_kann_keine_Taetigkeit_zugewiesen_werden()
    {
        var vorname = "Alan";
        var nachname = "Turing";
        var gruppe = Gruppe.Create("Frühschicht");
        var taetigkeit = Taetigkeit.Create("Einlagern");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value).Value;

        var result = mitarbeiter.WeiseTaetigkeitZu(taetigkeit.Value);

        result.Should().Fail();
    }
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_für_eine_Tätigkeit_qualifiziert_wird_kann_diese_Tätigkeit_zugewiesen_werden()
    {
        var vorname = "Alan";
        var nachname = "Turing";
        
        var gruppe = Gruppe.Create("Frühschicht");
        var taetigkeit = Taetigkeit.Create("Einlagern");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value);

        mitarbeiter.Value.QualifiziereFuer(taetigkeit.Value);
        
        var result = mitarbeiter.Value.WeiseTaetigkeitZu(taetigkeit.Value);

        result.Should().Succeed();

        mitarbeiter.Value.ZugewieseneTaetigkeiten.Should().HaveCount(1);
    }
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_für_eine_Tätigkeit_qualifiziert_ist_kann_die_gleiche_Taetigkeit_nicht_noch_einmal_qualifiziert_werden()
    {
        var vorname = "Alan";
        var nachname = "Turing";
        
        var gruppe = Gruppe.Create("Frühschicht");
        var taetigkeit = Taetigkeit.Create("Einlagern");

        var mitarbeiter = Mitarbeiter.Create(vorname, nachname, gruppe.Value).Value;

        var qualifizierung = mitarbeiter.QualifiziereFuer(taetigkeit.Value);
        var doppelteQualifizierung = mitarbeiter.QualifiziereFuer(taetigkeit.Value);

        qualifizierung.Should().Succeed();
        doppelteQualifizierung.Should().Fail();
    }

    [Fact]
    public void
        Wenn_zwei_Mitarbeiter_den_selben_Vornamen_und_Nachnamen_haben_sowie_in_der_gleichen_Gruppe_arbeiten_handelt_es_sich_um_den_selben_Mitarbeiter()
    {
        var vorname = "Alan";
        var nachname = "Turing";

        var gruppe = Gruppe.Create("Frühschicht");

        var alan = Mitarbeiter.Create(vorname, nachname, gruppe.Value);
        var auchAlan = Mitarbeiter.Create(vorname, nachname, gruppe.Value);

        var istDieSelbePerson = alan.Value.Equals(auchAlan.Value);

        istDieSelbePerson.Should().BeTrue();
    }
}