using MongoDB.Driver;

namespace Learnify_backend.Services.MongoDbService
{
    public interface IMongoDbService
    {
        IMongoDatabase Database { get; }
    }
}
