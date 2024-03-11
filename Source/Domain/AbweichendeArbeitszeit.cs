using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace Perosnaldisposition;

public class AbweichendeArbeitszeit
{
    public DayOfWeek Tag { get; }
    public TimeOnly Beginn { get; }
    public TimeOnly Ende { get; }

    [JsonConstructor]
    private AbweichendeArbeitszeit(DayOfWeek tag, TimeOnly beginn, TimeOnly ende)
    {
        Tag = tag;
        Beginn = beginn;
        Ende = ende;
    }

    public static Result<AbweichendeArbeitszeit> Create(DayOfWeek tag, TimeOnly beginn, TimeOnly ende)
    {
        if (tag is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return
                Result.Failure<AbweichendeArbeitszeit>("Für das Wochenende kann keine Abweichende Arbeitszeit erzeugt werden");

        if (ende < beginn)
            return
                Result.Failure<AbweichendeArbeitszeit>("Das Arbeitsende darf nicht vor dem Arbeitsbeginn liegen.");

        return Result.Success(new AbweichendeArbeitszeit(tag, beginn, ende));
    }
}