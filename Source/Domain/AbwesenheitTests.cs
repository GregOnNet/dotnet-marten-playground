using FluentAssertions;

namespace Perosnaldisposition;

public class AbwesenheitTests
{
    [Fact]
    public void Wenn_eine_Abwesenheit_in_der_Vergangenheit_angelegt_wird_schlägt_dies_fehl()
    {
        var gestern = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());


        var abwesenheit = Abwesenheit.Create(gestern, mitarbeiter.Value);

        abwesenheit.Should().Fail();
    }

    [Fact]
    public void Wenn_eine_Abwesenheit_ohne_Mitarbeiter_angelegt_wird_schlägt_dies_fehl()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());

        mitarbeiter.Value.Id = Guid.Empty; // Simulate invalid Id

        var abwesenheit = Abwesenheit.Create(heute, mitarbeiter.Value);

        abwesenheit.Should().Fail();
    }

    [Fact]
    public void Wenn_eine_Abwesenheit_mit_gültigen_Informationen_erzeugt_wird_gelingt_dies()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var mitarbeiter = Mitarbeiter.Create("Alan", "Turing", Guid.NewGuid());

        var abwesenheit = Abwesenheit.Create(heute, mitarbeiter.Value);

        abwesenheit.Should().Succeed();
    }
}