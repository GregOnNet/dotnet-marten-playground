using CSharpFunctionalExtensions;
using FluentAssertions;

namespace Perosnaldisposition;

public class Mitarbeiter
{
    private readonly string _vorname;
    private readonly string _nachname;

    private Mitarbeiter(string vorname, string nachname)
    {
        _vorname = vorname;
        _nachname = nachname;
    }
    
    public static Result<Mitarbeiter> Create(string vorname, string nachname)
    {
        if (string.IsNullOrWhiteSpace(vorname))
        {
            return Result.Failure<Mitarbeiter>("Bitte geben Sie einen Vornamen an.");
        }
        
        if (string.IsNullOrWhiteSpace(nachname))
        {
            return Result.Failure<Mitarbeiter>("Bitte geben Sie einen Nachnamen an.");
        }
        
        return Result.Success(new Mitarbeiter(vorname, nachname));
    }
}

public class MitarbeiterTests
{
    [Fact]
    public void Wenn_ein_Mitarbeiter_ohne_Vornamen_angelegt_wird_schlägt_dies_fehl()
    {
        var vorname = "";
        var nachname = "";
        
        var result = Mitarbeiter.Create(vorname, nachname);

        result.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public void Wenn_ein_Mitarbeiter_ohne_Nachnamen_angelegt_wird_schlägt_dies_fehl()
    {
        var vorname = "Elon";
        var nachname = "";
        
        var result = Mitarbeiter.Create(vorname, nachname);

        result.IsFailure.Should().BeTrue();
    }
}