using MGR.WPF.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGR.WPF.MethodsServices.Filters
{
    public class PearsonCorrelation
    {
        private DatabaseService databaseService;
        public PearsonCorrelation()
        {
            this.databaseService = new DatabaseService();
        }
        public double CompereTwoFeatures(List<Int32> featureX, List<Int32> featureY)
        {
            List<IDictionary<String, double>> featuresScore = new List<IDictionary<String, double>>();
            var avgX = featureX.Average(x => x);
            var avgY = featureY.Average(y => y);
            //var ttt = featureX.Select(x => x.["Column1"]);
            double sumUP = 0.00;
            double sumDWX = 0.00;
            double sumDWY = 0.00;
            for (int i = 0; i < featureX.Count(); i++)
            {
                sumUP += (featureX[i] - avgX) * (featureY[i] - avgY);
                sumDWX += Math.Pow((featureX[i] - avgX), 2);
                sumDWY += Math.Pow((featureY[i] - avgY), 2);
            }
            return sumUP / (Math.Sqrt(sumDWX) * Math.Sqrt(sumDWY));
        }
           
        public double[,] MakeCorelationTable(int featuresCount, string collectionName)
        {
            var dataSet = databaseService.ConvertMongoColectionToListOfLists(featuresCount, collectionName);
            double[,] corelationArray = new double[featuresCount, featuresCount];
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
            //for (int i = 1; i <= featuresCount; i++)
            //{
            //    Stopwatch stopWatch1 = new Stopwatch();
            //    stopWatch1.Start();
            //    var listX = databaseService.Get(collectionName, $"Column{i}");

            //    for (int j = 1; j <= featuresCount; j++)
            //    {
            //        var listY = databaseService.Get(collectionName, $"Column{j}");
            //        corelationArray[i,j] = CompereTwoFeatures(listX.Select(x => x[$"Column{i}"]).ToList(), listY.Select(x => x[$"Column{j}"]).ToList());
            //    }
            //    stopWatch1.Stop();
            //}
            //stopWatch.Stop();

            List<int> numbers = new List<int>();
            for (int i = 0; i < featuresCount; i++)
            {
                numbers.Add(i);
            }

            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
            //Parallel.ForEach(numbers, (elementX) =>
            //{
            //    var listX = databaseService.Get(collectionName, $"Column{elementX}");
            //    Parallel.ForEach(numbers, (elementY) =>
            //    {
            //        var listY = databaseService.Get(collectionName, $"Column{elementY}");
            //        corelationArray[elementX, elementY] = CompereTwoFeatures(listX.Select(x => x[$"Column{elementX}"]).ToList(), listY.Select(x => x[$"Column{elementY}"]).ToList());
            //    });
            //});
            //stopWatch.Stop();

            Parallel.ForEach(numbers, (elementX) =>
            {
                var listX = dataSet[elementX + 1];
                for (int i = 0; i < featuresCount; i++)
                {
                    corelationArray[elementX, i] = CompereTwoFeatures(listX, dataSet[i + 1]);
                }
            });

            return corelationArray;
        }
        
    }
}
