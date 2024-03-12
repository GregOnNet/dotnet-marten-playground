﻿using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Perosnaldisposition;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Plaene;

public class NeuenPlanErfassenEndpoint
{
    public static async Task<IResult> Handle([FromBody] CreatePlanRequest create,
                                             [FromServices] IDocumentSession session)
    {
        var planFuerAngegebenenTag = await session.Query<Plan>().Where(plan => plan.Tag == create.Tag).ToListAsync();
        
        if (planFuerAngegebenenTag.Any())
            return Results.Conflict($"Der Plan kann nicht angelegt werden, weil er für den \"{create.Tag.ToShortDateString()}\" bereits existiert.");
        
        return await Plan.Create(create.Tag)
                         .Map(async plan =>
                              {
                                  session.Store(plan);

                                  await session.SaveChangesAsync();

                                  return new CreatePlanResponse(plan.Id);
                              })
                         .Finally(result => result.IsSuccess
                                                ? Results.CreatedAtRoute(nameof(NeuenPlanErfassenEndpoint),
                                                                         result.Value.Id, result.Value)
                                                : Results.BadRequest(result.Error));
    }
}

public record struct CreatePlanRequest(DateOnly Tag);

public record struct CreatePlanResponse(Guid Id);