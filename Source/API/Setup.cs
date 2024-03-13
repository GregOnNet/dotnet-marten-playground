using Marten;
using Marten.Services.Json;
using Weasel.Core;

namespace API;

public class Setup
{
    public static void Marten(IHostEnvironment environment, IServiceCollection services, string connectionString)
    {
        services.AddMarten(options =>
                           {
                               options.Connection(connectionString);

                               options.UseDefaultSerialization(
                                                               serializerType: SerializerType.SystemTextJson,
                                                               collectionStorage: CollectionStorage.AsArray,
                                                               nonPublicMembersStorage: NonPublicMembersStorage.All
                                                              );
                               
                               if (environment.IsDevelopment())
                                   options.AutoCreateSchemaObjects = AutoCreate.All;
                           }).UseLightweightSessions();
    }
}