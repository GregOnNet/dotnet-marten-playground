using FluentAssertions;

namespace Perosnaldisposition;

public class DispositionTests
{
    [Fact]
    public void
        Wenn_ein_Mitarbeiter_für_eine_Tätigkeit_nicht_qualifiziert_ist_kann_die_Disposition_nicht_erstellt_werden()
    {
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());
        var taetigkeit = Taetigkeit.Create("Einlagern");

        var disposition = Disposition.Create(mitarbeiter.Value, taetigkeit.Value);

        disposition.Should().Fail();
    }
    
    [Fact]
    public void
        Wenn_ein_Mitarbeiter_für_eine_Tätigkeit_qualifiziert_ist_kann_die_Disposition_erstellt_werden()
    {
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());
        var taetigkeit = Taetigkeit.Create("Einlagern");

        mitarbeiter.Value.Qualifiziere(taetigkeit.Value);

        var disposition = Disposition.Create(mitarbeiter.Value, taetigkeit.Value);

        disposition.Should().Succeed();
    }
    
    [Fact]
    public void
        Wenn_eine_Disposition_mehrere_Taetigkeiten_enthält_werden_diese_in_der_gegebenen_Reihenfolge_hinterlegt ()
    {
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());
        var einlagern = Taetigkeit.Create("Einlagern");
        var kommissionieren = Taetigkeit.Create("Kommissionieren");

        mitarbeiter.Value.Qualifiziere(einlagern.Value);
        mitarbeiter.Value.Qualifiziere(kommissionieren.Value);

        var disposition = Disposition.Create(mitarbeiter.Value, [einlagern.Value, kommissionieren.Value]);

        disposition.Should().Succeed();

        disposition.Value.TaetigkeitenIds.First().Should().Be(einlagern.Value.Id);
        disposition.Value.TaetigkeitenIds.Last().Should().Be(kommissionieren.Value.Id);
    }
    
    [Fact]
    public void
        Wenn_ein_Mitarbeiter_nicht_für_alle_gegebenen_Tätigkeiten_qualifiziert_ist_kann_keine_Disposition_erzeugt_werden()
    {
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());
        var einlagern = Taetigkeit.Create("Einlagern");
        var kommissionieren = Taetigkeit.Create("Kommissionieren");

        mitarbeiter.Value.Qualifiziere(einlagern.Value);

        var disposition = Disposition.Create(mitarbeiter.Value, [einlagern.Value, kommissionieren.Value]);

        disposition.Should().Fail();
    }
}