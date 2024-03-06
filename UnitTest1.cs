namespace Perosnaldisposition;

public class UnitTest1
{
    [Fact]
    public void Wenn_ein_Mitarbeiter_ohne_Vornamen_angelegt_wird_schlägt_dies_fehl()
    {
        var vorname = "";
        var mitarbeiter = Mitarbeiter.Create(vorname);
        
        
    }
}