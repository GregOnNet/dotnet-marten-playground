using CSharpFunctionalExtensions;

namespace Perosnaldisposition;

public class Plan
{
    public DateOnly Tag { get; }

    public IEnumerable<Disposition> Dispositionen { get; private set; }

    private Plan(DateOnly tag)
    {
        Tag = tag;
        Dispositionen = Array.Empty<Disposition>();
    }

    public static Result<Plan> Create(DateOnly tag)
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);

        if (tag < heute)
        {
            return Result.Failure<Plan>("Ein Plan kann nicht für einen vergangenen Tag angelegt werden.");
        }
        
        return Result.Success(new Plan(tag));
    }

    public Result Disponiere(Disposition neueDisposition)
    {
        if (Dispositionen.Any(disposition => disposition.Mitarbeiter.Equals(neueDisposition.Mitarbeiter)))
        {
            return Result.Failure($"Der Mitarbeiter {neueDisposition.Mitarbeiter.Vorname} {neueDisposition.Mitarbeiter.Nachname} kann nicht mehrfach disponiert werden.");
        }
        
        Dispositionen = Dispositionen.Append(neueDisposition);
        
        return Result.Success();
    }
}