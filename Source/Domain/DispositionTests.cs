using FluentAssertions;

namespace Perosnaldisposition;

public class DispositionTests
{
    [Fact]
    public void
        Wenn_ein_Mitarbeiter_für_eine_Tätigkeit_nicht_qualifiziert_ist_kann_die_Disposition_nicht_erstellt_werden()
    {
        var gruppe = Gruppe.Create("Frühschicht");
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", gruppe.Value);
        var taetigkeit = Taetigkeit.Create("Einlagern");

        var disposition = Disposition.Create(mitarbeiter.Value, taetigkeit.Value);

        disposition.Should().Fail();
    }
    
    [Fact]
    public void
        Wenn_ein_Mitarbeiter_für_eine_Tätigkeit_qualifiziert_ist_kann_die_Disposition_erstellt_werden()
    {
        var gruppe = Gruppe.Create("Frühschicht");
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", gruppe.Value);
        var taetigkeit = Taetigkeit.Create("Einlagern");

        mitarbeiter.Value.QualifiziereFuer(taetigkeit.Value);

        var disposition = Disposition.Create(mitarbeiter.Value, taetigkeit.Value);

        disposition.Should().Succeed();
    }
}