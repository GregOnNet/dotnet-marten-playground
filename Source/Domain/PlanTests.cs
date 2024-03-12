using FluentAssertions;

namespace Perosnaldisposition;

public class PlanTests
{
    [Fact]
    public void Wenn_ein_Plan_mit_einem_vergangenen_Tag_angelegt_wird_schlägt_dies_fehl()
    {
        var plan = Plan.Create(DateOnly.MinValue);

        plan.Should().Fail();
    }

    [Fact]
    public void Wenn_ein_Plan_für_heute_angelegt_wird_gelingt_dies()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var plan = Plan.Create(heute);

        plan.Should().Succeed();
        plan.Value.Tag.Should().Be(heute);
    }

    [Fact]
    public void Wenn_ein_Plan_für_die_Zukunft_angelegt_wird_gelingt_dies()
    {
        var in2Tagen = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        var plan = Plan.Create(in2Tagen);

        plan.Should().Succeed();
        plan.Value.Tag.Should().Be(in2Tagen);
    }

    [Fact]
    public void Wenn_ein_Plan_angelegt_ist_kann_eine_Disposition_hinzugefügt_werden()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var plan = Plan.Create(heute);

        var taetigkeit = Taetigkeit.Create("Einlagern");
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());

        mitarbeiter.Value.Qualifiziere(taetigkeit.Value);

        var disposition = Disposition.Create(mitarbeiter.Value, taetigkeit.Value);

        plan.Value.Disponiere(disposition.Value);
        plan.Value.Dispositionen.Should().HaveCount(1);
    }

    [Fact]
    public void
        Wenn_eine_Disposition_mit_einer_Tätigkeit_existiert_und_für_den_selben_Mitarbeiter_eine_weitere_Disposition_mit_einer_anderen_Tätigkeit_eingeht_wird_die_bisherige_Tätigkeit_überschrieben()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var plan = Plan.Create(heute);

        var einlagern = Taetigkeit.Create("Einlagern");
        var kommissionieren = Taetigkeit.Create("Kommissionieren");
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());

        mitarbeiter.Value.Qualifiziere(einlagern.Value);
        plan.Value.Disponiere(Disposition.Create(mitarbeiter.Value, einlagern.Value).Value);

        mitarbeiter.Value.Qualifiziere(kommissionieren.Value);
        plan.Value.Disponiere(Disposition.Create(mitarbeiter.Value, kommissionieren.Value).Value);


        plan.Value.Dispositionen.Should().HaveCount(1);
        plan.Value.Dispositionen[mitarbeiter.Value.Id].TaetigkeitenIds.First().Should().Be(kommissionieren.Value.Id);
    }

    [Fact]
    public void
        Wenn_eine_Disposition_mit_2_Tätigkeiten_existiert_und_für_den_selben_Mitarbeiter_eine_weitere_Disposition_mit_einer_anderen_Tätigkeit_eingeht_wird_die_erste_der_bereits_disponierten_Tätigkeiten_überschrieben()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var plan = Plan.Create(heute);

        var einlagern = Taetigkeit.Create("Einlagern");
        var kommissionieren = Taetigkeit.Create("Kommissionieren");
        var langgut = Taetigkeit.Create("Langgut");
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());

        mitarbeiter.Value.Qualifiziere(einlagern.Value);
        mitarbeiter.Value.Qualifiziere(kommissionieren.Value);

        plan.Value.Disponiere(Disposition.Create(mitarbeiter.Value, [einlagern.Value, kommissionieren.Value]).Value);

        mitarbeiter.Value.Qualifiziere(langgut.Value);
        plan.Value.Disponiere(Disposition.Create(mitarbeiter.Value, langgut.Value).Value);

        plan.Value.Dispositionen[mitarbeiter.Value.Id].TaetigkeitenIds.First().Should().Be(langgut.Value.Id);
        plan.Value.Dispositionen[mitarbeiter.Value.Id].TaetigkeitenIds.Last().Should().Be(kommissionieren.Value.Id);
    }

    [Fact]
    public void
        Wenn_eine_Disposition_mit_2_Tätigkeiten_existiert_und_für_den_selben_Mitarbeiter_eine_weitere_Disposition_mit_der_Folgetätigkeit_eingeht_gilt_die_erste_Tätigkeit_als_erledigt()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var plan = Plan.Create(heute);

        var einlagern = Taetigkeit.Create("Einlagern");
        var kommissionieren = Taetigkeit.Create("Kommissionieren");
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());

        mitarbeiter.Value.Qualifiziere(einlagern.Value);
        mitarbeiter.Value.Qualifiziere(kommissionieren.Value);

        plan.Value.Disponiere(Disposition.Create(mitarbeiter.Value, [einlagern.Value, kommissionieren.Value]).Value);

        plan.Value.Disponiere(Disposition.Create(mitarbeiter.Value, kommissionieren.Value).Value);

        plan.Value.Dispositionen[mitarbeiter.Value.Id].TaetigkeitenIds.Should().HaveCount(1);
        plan.Value.Dispositionen[mitarbeiter.Value.Id].TaetigkeitenIds.First().Should().Be(kommissionieren.Value.Id);
    }

    [Fact]
    public void
        Wenn_eine_Disposition_mit_2_Tätigkeiten_existiert_und_für_den_selben_Mitarbeiter_eine_weitere_Disposition_mit_mehreren_Tätigkeiten_eingeht_werden_die_bisherigen_Tätigkeiten_überschrieben()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var plan = Plan.Create(heute);

        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());

        var einlagern = Taetigkeit.Create("Einlagern");
        var kommissionieren = Taetigkeit.Create("Kommissionieren");

        mitarbeiter.Value.Qualifiziere(einlagern.Value);
        mitarbeiter.Value.Qualifiziere(kommissionieren.Value);

        plan.Value.Disponiere(Disposition.Create(mitarbeiter.Value, [einlagern.Value, kommissionieren.Value]).Value);

        var langgut = Taetigkeit.Create("Langgut");
        var abholung = Taetigkeit.Create("Abholung");

        mitarbeiter.Value.Qualifiziere(langgut.Value);
        mitarbeiter.Value.Qualifiziere(abholung.Value);

        plan.Value.Disponiere(Disposition.Create(mitarbeiter.Value, [langgut.Value, abholung.Value]).Value);

        plan.Value.Dispositionen[mitarbeiter.Value.Id].TaetigkeitenIds.First().Should().Be(langgut.Value.Id);
        plan.Value.Dispositionen[mitarbeiter.Value.Id].TaetigkeitenIds.Last().Should().Be(abholung.Value.Id);
    }

    [Fact]
    public void
        Wenn_für_die_Planung_die_Abwesenheit_eines_Mitarbeiters_berücksichtigt_werden_soll_wird_diese_hinterlegt()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var plan = Plan.Create(heute);
        var alan = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());

        var planMitAbwesenheit = plan.Value.BeruecksichtigeAbwesenheitVon(alan.Value);

        planMitAbwesenheit.Should().Succeed();

        planMitAbwesenheit.Value.AbwesendeMitarbeiterIds.Should().HaveCount(1);
    }

    [Fact]
    public void
        Wenn_für_die_Planung_die_Abwesenheit_eines_Mitarbeiters_mehrfach_hinterlegt_wird_existiert_dennoch_nur_ein_Abwesenheitseintrag()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var plan = Plan.Create(heute);
        var alan = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());

        var planMitAbwesenheit = plan.Value.BeruecksichtigeAbwesenheitVon(alan.Value);
        var planMitAbwesenheitFuerGleichenTagNochmal = plan.Value.BeruecksichtigeAbwesenheitVon(alan.Value);

        planMitAbwesenheit.Should().Succeed();
        planMitAbwesenheitFuerGleichenTagNochmal.Should().Succeed();

        planMitAbwesenheitFuerGleichenTagNochmal.Value.AbwesendeMitarbeiterIds.Should().HaveCount(1);
    }

    [Fact]
    public void Wenn_ein_Mitarbeiter_abwesend_ist_kann_dieser_nicht_disponiert_werden()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var plan = Plan.Create(heute);
        var alan = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());
        var taetigkeit = Taetigkeit.Create("Einlagern");

        alan.Value.Qualifiziere(taetigkeit.Value);
        plan.Value.BeruecksichtigeAbwesenheitVon(alan.Value);

        var disposition = Disposition.Create(alan.Value, taetigkeit.Value);

        var planMitDisposition = plan.Value.Disponiere(disposition.Value);

        planMitDisposition.Should().Fail();
    }
}