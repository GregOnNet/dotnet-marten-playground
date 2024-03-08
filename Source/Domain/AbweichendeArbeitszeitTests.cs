using FluentAssertions;

namespace Perosnaldisposition;

public class AbweichendeArbeitszeitTests
{
    [Theory]
    [InlineData(DayOfWeek.Saturday)]
    [InlineData(DayOfWeek.Sunday)]
    public void Wenn_eine_abweichende_Arbeitszeit_am_Wochenende_erzeugt_werden_soll_schlägt_dies_fehl(
        DayOfWeek dayOfWeekend)
    {
        var beginn = TimeOnly.FromDateTime(DateTime.Today.AddHours(10));
        var ende = TimeOnly.FromDateTime(DateTime.Today.AddHours(9));

        var abweichendeArbeitszeit = AbweichendeArbeitszeit.Create(dayOfWeekend, beginn, ende);

        abweichendeArbeitszeit.Should().Fail();
    }

    [Fact]
    public void Wenn_eine_abweichende_Arbeitszeit_erzeugt_wird_darf_das_Arbeitsende_nicht_vor_dem_Arbeitsbeginn_sein()
    {
        var beginn = TimeOnly.FromDateTime(DateTime.Today.AddHours(10));
        var ende = TimeOnly.FromDateTime(DateTime.Today.AddHours(9));

        var abweichendeArbeitszeit = AbweichendeArbeitszeit.Create(DayOfWeek.Monday, beginn, ende);

        abweichendeArbeitszeit.Should().Fail();
    }

    [Fact]
    public void Wenn_eine_abweichende_Arbeitszeit_Werktags_mit_korrekten_Zeitangaben_erzeugt_wird_gelingt_dies()
    {
        var beginn = TimeOnly.FromDateTime(DateTime.Today.AddHours(8));
        var ende = TimeOnly.FromDateTime(DateTime.Today.AddHours(16));

        var abweichendeArbeitszeit = AbweichendeArbeitszeit.Create(DayOfWeek.Monday, beginn, ende);

        abweichendeArbeitszeit.Should().Succeed();

        abweichendeArbeitszeit.Value.Tag.Should().Be(DayOfWeek.Monday);
        abweichendeArbeitszeit.Value.Beginn.Should().Be(beginn);
        abweichendeArbeitszeit.Value.Ende.Should().Be(ende);
    }
}