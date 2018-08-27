using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGR.WPF.DatabaseServices
{
    public class DatabaseService
    {
        private MongoClient mongoClient;
        private IMongoDatabase mongoDatabase;
        public DatabaseService()
        {
            mongoClient = new MongoClient("mongodb://localhost:27017");
            mongoDatabase = mongoClient.GetDatabase("MGR");
        }

        public IMongoQueryable<BsonDocument> Get(string collectionName)
        {
            return mongoDatabase.GetCollection<BsonDocument>(collectionName).AsQueryable();  
        }

        public List<IDictionary<String, Int32>> Get(string collectionName, string columnName)
        {
            var cos =  mongoDatabase.GetCollection<KeyValuePair<string,string>>(collectionName).Find(Builders<KeyValuePair<string, string>>.Filter.Empty).Project(Builders<KeyValuePair<string, string>>.Projection.Include(columnName).Exclude("_id")).ToList();
            
            List<IDictionary<String, Int32>> keyValuePairs = new List<IDictionary<String, Int32>>();
            foreach (var item in cos)
            {
                keyValuePairs.Add(BsonSerializer.Deserialize<IDictionary<String, Int32>>(item));
            }
            var sasadas = keyValuePairs[0][columnName];

            
            return keyValuePairs;
        }


    }
}
