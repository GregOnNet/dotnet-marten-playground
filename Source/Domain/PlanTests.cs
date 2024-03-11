using System.Net.Mime;
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
        
        mitarbeiter.Value.QualifiziereFuer(taetigkeit.Value);
        
        var disposition = Disposition.Create(mitarbeiter.Value, taetigkeit.Value);

        plan.Value.Disponiere(disposition.Value);
        plan.Value.Dispositionen.Should().HaveCount(1);
    }
    
    [Fact]
    public void Wenn_ein_Plan_angelegt_eine_Disposition_fuer_einen_Mitarbeiter_enthält_kann_der_Mitarbeiter_nicht_erneut_disponiert_werden()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var plan = Plan.Create(heute);

        var taetigkeit = Taetigkeit.Create("Einlagern");
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());
        
        mitarbeiter.Value.QualifiziereFuer(taetigkeit.Value);
        
        var disposition = Disposition.Create(mitarbeiter.Value, taetigkeit.Value);

        var ersteDisposition = plan.Value.Disponiere(disposition.Value);
        var gleicheDispositionNochEinmal = plan.Value.Disponiere(disposition.Value);

        ersteDisposition.Should().Succeed();
        gleicheDispositionNochEinmal.Should().Fail();
        
        plan.Value.Dispositionen.Should().HaveCount(1);
    }
}