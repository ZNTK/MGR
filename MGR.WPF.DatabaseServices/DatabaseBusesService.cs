using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGR.WPF.DatabaseServices
{
    public class DatabaseBusesService
    {
        private MongoClient mongoClient;
        private IMongoDatabase mongoDatabase;
        public DatabaseBusesService()
        {
            mongoClient = new MongoClient("mongodb://localhost:27017");
            mongoDatabase = mongoClient.GetDatabase("ZBD");
        }
    }
}
