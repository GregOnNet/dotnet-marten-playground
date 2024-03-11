using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace Perosnaldisposition;

public class Abwesenheit
{
    [JsonConstructor]
    private Abwesenheit(DateOnly tag, Guid mitarbeiterId)
    {
        Id = Guid.NewGuid();
        Tag = tag;
        MitarbeiterId = mitarbeiterId;
    }

    public Guid Id { get; set; }
    public DateOnly Tag { get; }
    public Guid MitarbeiterId { get; }

    public static Result<Abwesenheit> Create(DateOnly tag, Mitarbeiter mitarbeiter)
    {
        if (tag < DateOnly.FromDateTime(DateTime.Today))
            return Result.Failure<Abwesenheit>("Es kann keine Abwesenheit für die Vergangenheit hinterlegt werden");

        if (mitarbeiter.Id == Guid.Empty)
            return
                Result.Failure<Abwesenheit>($"Die Id des Mitarbeiters {mitarbeiter.Vorname} {mitarbeiter.Nachname} (\"{mitarbeiter.Id}\") ist ungültig.");


        return Result.Success(new Abwesenheit(tag, mitarbeiter.Id));
    }
}