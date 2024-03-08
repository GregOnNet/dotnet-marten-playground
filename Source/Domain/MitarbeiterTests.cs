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
        mitarbeiter.Value.Gruppe.Name.Should().Be(gruppe.Value.Name);
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
    public void
        Wenn_ein_Mitarbeiter_für_eine_Tätigkeit_qualifiziert_ist_kann_die_gleiche_Taetigkeit_nicht_noch_einmal_qualifiziert_werden()
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

    [Fact]
    public void
        Wenn_zwei_Mitarbeiter_den_selben_Vornamen_und_Nachnamen_haben_aber_in_verschiedenen_Gruppen_arbeiten_handelt_es_sich_um_verschiedene_Mitarbeiter()
    {
        var vorname = "Alan";
        var nachname = "Turing";

        var fruehschicht = Gruppe.Create("Frühschicht");
        var spaetschicht = Gruppe.Create("Spätschicht");

        var alan = Mitarbeiter.Create(vorname, nachname, fruehschicht.Value);
        var auchAlan = Mitarbeiter.Create(vorname, nachname, spaetschicht.Value);

        var istDieSelbePerson = alan.Value.Equals(auchAlan.Value);

        istDieSelbePerson.Should().BeFalse();
    }

    [Fact]
    public void
        Wenn_ein_Mitarbeiter_an_einem_Tag_eine_von_seiner_Gruppe_abweichende_Arbeitszeit_möchte_kann_sie_vereinbart_werden()
    {
        var vorname = "Alan";
        var nachname = "Turing";

        var fruehschicht = Gruppe.Create("Frühschicht");

        var alan = Mitarbeiter.Create(vorname, nachname, fruehschicht.Value);

        var montagsVon8Bis14Uhr =
            AbweichendeArbeitszeit.Create(DayOfWeek.Monday, TimeOnly.Parse("08:00:00"), TimeOnly.Parse("14:00:00"));

        alan.Value.Vereinbare(montagsVon8Bis14Uhr.Value);

        alan.Value.AbweichendeArbeitszeiten.Should().HaveCount(1);
    }
    
    [Fact]
    public void
        Wenn_für_einen_Mitarbeiter_eine_abweichende_Arbeitszeit_vereinbart_wurde_und_diese_verändert_wird_gelingt_dies()
    {
        var vorname = "Alan";
        var nachname = "Turing";

        var fruehschicht = Gruppe.Create("Frühschicht");

        var alan = Mitarbeiter.Create(vorname, nachname, fruehschicht.Value);

        var montagsVon8Bis14Uhr =
            AbweichendeArbeitszeit.Create(DayOfWeek.Monday, TimeOnly.Parse("08:00:00"), TimeOnly.Parse("14:00:00"));

        var montagsVon6Bis12Uhr =
            AbweichendeArbeitszeit.Create(DayOfWeek.Monday, TimeOnly.Parse("06:00:00"), TimeOnly.Parse("12:00:00"));

        
        alan.Value.Vereinbare(montagsVon8Bis14Uhr.Value);
        alan.Value.Vereinbare(montagsVon6Bis12Uhr.Value);

        alan.Value.AbweichendeArbeitszeiten[DayOfWeek.Monday].Beginn.Should().Be(TimeOnly.Parse("06:00:00"));
    }
    
    [Fact]
    public void
        Wenn_für_einen_Mitarbeiter_eine_abweichende_Arbeitszeit_vereinbart_wurde_und_diese_gelöst_wird_gelingt_dies()
    {
        var vorname = "Alan";
        var nachname = "Turing";

        var fruehschicht = Gruppe.Create("Frühschicht");

        var alan = Mitarbeiter.Create(vorname, nachname, fruehschicht.Value);

        var montagsVon8Bis14Uhr =
            AbweichendeArbeitszeit.Create(DayOfWeek.Monday, TimeOnly.Parse("08:00:00"), TimeOnly.Parse("14:00:00"));

        alan.Value.Vereinbare(montagsVon8Bis14Uhr.Value);

        alan.Value.LoeseArbeitszeitVereinbarungFuer(montagsVon8Bis14Uhr.Value.Tag);

        alan.Value.AbweichendeArbeitszeiten.Should().BeEmpty();
    }
}