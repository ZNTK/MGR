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

        public List<IDictionary<String, double>> Get(string collectionName, string columnName)
        {
            var result = mongoDatabase.GetCollection<KeyValuePair<string, string>>(collectionName).Find(Builders<KeyValuePair<string, string>>.Filter.Empty).Project(Builders<KeyValuePair<string, string>>.Projection.Include(columnName).Exclude("_id")).ToList();

            List<IDictionary<String, double>> keyValuePairs = new List<IDictionary<String, double>>();
            foreach (var item in result)
            {
                keyValuePairs.Add(BsonSerializer.Deserialize<IDictionary<String, double>>(item));
            }


            return keyValuePairs;
        }

        public List<List<double>> ConvertMongoColectionToListOfLists(int featuresCount, string collectionName)
        {
            List<List<double>> listOfLists = new List<List<double>>();
            for (int i = 0; i < featuresCount; i++)
            {
                var column = Get(collectionName, $"Column{i+1}");
                listOfLists.Add(column.Select(x => (double)x[$"Column{i + 1}"]).ToList());
            }
            return listOfLists;
        }

        //public List<IDictionary<String, Int32>> GetWithID(string collectionName, string columnName)
        //{
        //    var result = mongoDatabase.GetCollection<KeyValuePair<string, string>>(collectionName).Find(Builders<KeyValuePair<string, string>>.Filter.Empty).Project(Builders<KeyValuePair<string, string>>.Projection.Include(columnName)).ToList();

        //    List<KeyPairWithId> keyValuePairs = new List<KeyPairWithId>();
        //    foreach (var item in result)
        //    {
        //        keyValuePairs.Add(BsonSerializer.Deserialize<IDictionary<String, Int32>>(item));
        //    }


        //    return keyValuePairs;
        //}


    }

    public class KeyPairWithId
    {
        public Guid id { get; set; }
        public IDictionary<String, Int32> keyValuePair { get; set; }

        public KeyPairWithId(Guid id, IDictionary<String, Int32> keyValuePair)
        {
            this.id = id;
            this.keyValuePair = keyValuePair;
        }
    }
}
