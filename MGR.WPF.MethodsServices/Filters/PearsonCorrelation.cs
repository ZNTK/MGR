using MGR.WPF.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public double CompereTwoFeatures(List<double> featureX, List<double> featureY)
        {
            var avgX = featureX.Average(x => x);
            var avgY = featureY.Average(y => y);
            double sumUP = 0.00;
            double sumDWX = 0.00;
            double sumDWY = 0.00;
            for (int i = 0; i < featureX.Count(); i++)
            {
                sumUP += (featureX[i] - avgX) * (featureY[i] - avgY);
                sumDWX += Math.Pow((featureX[i] - avgX), 2);
                sumDWY += Math.Pow((featureY[i] - avgY), 2);
            }
            if(sumUP == 0)
            {
                return 0.00;
            }
            return sumUP / (Math.Sqrt(sumDWX) * Math.Sqrt(sumDWY));
        }
           
        public double[,] MakeCorelationTable(int featuresCount, string collectionName)
        {
            Stopwatch stopWatchMakeTable = new Stopwatch();
            stopWatchMakeTable.Start();
            var dataSet = databaseService.ConvertMongoColectionToListOfLists(featuresCount, collectionName);
            stopWatchMakeTable.Stop();
            double[,] corelationArray = new double[featuresCount, featuresCount];
            int licznik = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Parallel.ForEach(dataSet, (list, state, index) =>
            {
                for (int i = (int)index + 1; i < featuresCount; i++)
                {
                    corelationArray[(int)index, i] = Math.Abs(CompereTwoFeatures(list, dataSet[i]));
                }
                licznik++;
                Console.WriteLine($"koniec numer {licznik} czas: {stopWatch.ElapsedMilliseconds}");
            });
            
            stopWatch.Stop();

            var times = new StringBuilder();

            times.AppendLine($"Czas tworzenia tabeli z  danymi pozyskanymi z MongoDB: {stopWatchMakeTable.ElapsedMilliseconds}");
            times.AppendLine($"Czas wykonywania sie algorytmu dla wszystkich zmiennych: {stopWatch.ElapsedMilliseconds}");

            FiltersHelper filtersHelper = new FiltersHelper();
            filtersHelper.GetTimesAndWriteToFile(times, collectionName, "Pearson");

            return corelationArray;
        }
        
    }
}
