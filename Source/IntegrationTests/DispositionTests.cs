using API.UseCases.Gruppen.Erfassen;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Perosnaldisposition;

namespace IntegrationTests;

[Collection("Database")]
public class DispositionTests
{
    private readonly DatabaseFixture _fixture;

    public DispositionTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Test1()
    {
        await using var session = _fixture.Store.LightweightSession();

        session.Store(Gruppe.Create("Hallo").Value);

        await session.SaveChangesAsync();
    }

    [Fact]
    public async Task Test2()
    {
        await using var session = _fixture.Store.LightweightSession();

        var result = await NeueGruppeErfassenEndpoint.Handle(new CreateGruppeRequest { Name = "Gruppe 2" }, session);

        var cr = result.Should()
                       .BeOfType<CreatedAtRoute<CreateGruppeResponse>>()
                       .Subject.Value.Id.Should().NotBeEmpty();

    }
}