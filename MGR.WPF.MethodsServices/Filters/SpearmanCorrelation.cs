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
            return 1 - ((6 * sumDi) / (featureX.Count * (Math.Pow(featureX.Count, 2) - 1)));
        }
        public double[,] MakeCorelationTable(int featuresCount, string collectionName)
        {
            Stopwatch stopWatchMakeTable = new Stopwatch();
            stopWatchMakeTable.Start();
            var dataSet = databaseService.ConvertMongoColectionToListOfLists(featuresCount, collectionName);

            stopWatchMakeTable.Stop();
            Stopwatch stopWatchMakeRankingTable = new Stopwatch();
            stopWatchMakeRankingTable.Start();
            FiltersHelper filtersHelper = new FiltersHelper();
            List<List<double>> rankDataSet = new List<List<double>>();
            foreach (var item in dataSet)
            {
                var result = filtersHelper.RankFeature(item);
                rankDataSet.Add(result);
            }
            stopWatchMakeRankingTable.Stop();


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
            stopWatch.Stop();

            var times = new StringBuilder();

            times.AppendLine($"Czas tworzenia tabeli z  danymi pozyskanymi z MongoDB: {stopWatchMakeTable.ElapsedMilliseconds}");
            times.AppendLine($"Czas tworzenia tabeli zrankingowanych danych: {stopWatchMakeRankingTable.ElapsedMilliseconds}");
            times.AppendLine($"Czas wykonywania sie algorytmu dla wszystkich zmiennych: {stopWatch.ElapsedMilliseconds}");
            
            filtersHelper.GetTimesAndWriteToFile(times, collectionName, "Spearman");

            return corelationArray;
        }
    }
}
 