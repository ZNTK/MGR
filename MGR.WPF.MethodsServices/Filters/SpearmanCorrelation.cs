using MGR.WPF.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGR.WPF.MethodsServices.Filters
{
    public class SpearmanCorrelation
    {
        private DatabaseService databaseService;
        public SpearmanCorrelation()
        {
            this.databaseService = new DatabaseService();
        }
        public double CompereTwoFeatures(List<double> featureX, List<double> featureY)
        {
            double sumDi = 0.00;
            for (int i = 0; i < featureX.Count; i++)
            {
                sumDi += Math.Pow((featureX[i] - featureY[i]), 2);
            }
            var result = 1 - ((6 * sumDi) / (featureX.Count * (Math.Pow(featureX.Count, 2) - 1)));

            if(result > 1 || result < -1)
            {
                var tak = 0;
            }
            return 1 - ((6 * sumDi) / (featureX.Count * (Math.Pow(featureX.Count, 2) - 1)));
        }
        public double[,] MakeCorelationTable(int featuresCount, string collectionName)
        {
            Stopwatch stopWatch1 = new Stopwatch();
            stopWatch1.Start();
            var dataSet = databaseService.ConvertMongoColectionToListOfLists(featuresCount, collectionName);


            FiltersHelper filtersHelper = new FiltersHelper();
            List<List<double>> rankDataSet = new List<List<double>>();
            foreach (var item in dataSet)
            {
                var result = filtersHelper.RankFeature(item);
                rankDataSet.Add(result);
            }

            stopWatch1.Stop();
            double[,] corelationArray = new double[featuresCount, featuresCount];
            
            //List<int> numbers = new List<int>();
            //for (int i = 0; i < featuresCount; i++)
            //{
            //    numbers.Add(i);
            //}
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Parallel.ForEach(rankDataSet, (rankList, state, index) =>
            {
                Console.WriteLine($"robie numer {index} czas: {stopWatch.ElapsedMilliseconds}");
                for (int i = (int)index + 1; i < featuresCount; i++)
                {
                    corelationArray[(int)index, i] = Math.Abs(CompereTwoFeatures(rankList, rankDataSet[i]));
                }
                Console.WriteLine($"koniec numer {index} czas: {stopWatch.ElapsedMilliseconds}");
            });


            double max = 0.00;
            string jakiXY = "";
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    if (i != j)
                    {
                        if (corelationArray[i, j] > max)
                        {
                            max = corelationArray[i, j];
                            jakiXY = $"{i} {j}";
                        }
                    }
                }
            }
            stopWatch.Stop();
            return corelationArray;
        }
    }
}
 