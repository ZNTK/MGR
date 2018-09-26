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
    public class KendallCorelation
    {
        private DatabaseService databaseService;
        public KendallCorelation()
        {
            this.databaseService = new DatabaseService();
        }
        public double CompereTwoFeatures(List<double> featureX, List<double> featureY)
        {
            List<TwoFeatures> twoFeatures = new List<TwoFeatures>();
            for (int i = 0; i < featureX.Count; i++)
            {
                twoFeatures.Add(new TwoFeatures(featureX[i], featureY[i]));
            }

            var sortedByX = twoFeatures.OrderBy(x => x.PropY).OrderBy(x => x.PropX).ToList();
            var sortedByY = twoFeatures.OrderBy(x => x.PropX).OrderBy(x => x.PropY).ToList();
            int sumC = 0;
            int sumD = 0;
            //int Tx = 0;
            //int Ty = 0;
            for (int i = 0; i < sortedByX.Count; i++)
            {
                int C = 0;
                int D = 0;
                for (int j = i + 1; j < sortedByX.Count; j++)
                {
                    if(sortedByX[j].PropY == 86)
                    {

                    }
                    if(sortedByX[i].PropX < sortedByX[j].PropY)
                    {
                        C++;
                    }
                    else
                    {
                        D++;
                    }
                }
                sumC += C;
                sumD += D;
            }
            //int licznik = 1;
            //for (int i = 1; i <= sortedByX.Count; i++)
            //{
            //    if(i != sortedByX.Count && sortedByX[i - 1].PropX == sortedByX[i].PropX)
            //    {
            //        licznik++;
            //    }
            //    else
            //    {
            //        if (licznik != 1)
            //        {
            //            Tx += licznik * (licznik - 1) / 2;
            //            licznik = 1;
            //        }
            //    }
            //}
            //for (int i = 1; i < sortedByY.Count; i++)
            //{
            //    if (i != sortedByY.Count && sortedByY[i - 1].PropY == sortedByY[i].PropY)
            //    {
            //        licznik++;
            //    }
            //    else
            //    {
            //        if (licznik != 1)
            //        {
            //            Ty += licznik * (licznik - 1) / 2;
            //            licznik = 1;
            //        }
            //    }
            //}
            //var Ns = (Math.Pow(twoFeatures.Count, 2) - twoFeatures.Count) / 2;
            //var result = (sumC - sumD - Tx - Ty) / Math.Sqrt((Ns - Tx) * (Ns - Ty));
            var result = (double)(sumC - sumD) / (sumC + sumD);
            return result;
        }

        public double[,] MakeCorelationTable(int featuresCount, string collectionName, List<List<double>> dataSet)
        {
            Stopwatch stopWatchMakeRankingTable = new Stopwatch();
            FiltersHelper filtersHelper = new FiltersHelper();
            List<List<double>> rankDataSet = new List<List<double>>();
            foreach (var item in dataSet)
            {
                var result = filtersHelper.RankFeature(item);
                rankDataSet.Add(result);
            }
            

            double[,] corelationArray = new double[featuresCount+1, featuresCount+1];
            

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int licznik = 0;
            Parallel.ForEach(rankDataSet, (rankList, state, index) =>
            {
                //Console.WriteLine($"robie numer {index} czas: {stopWatch.ElapsedMilliseconds}");
                for (int i = (int)index + 1; i < featuresCount+1; i++)
                {
                    corelationArray[(int)index, i] = Math.Abs(CompereTwoFeatures(rankList, rankDataSet[i]));
                }
                //Console.WriteLine($"koniec numer {index} czas: {stopWatch.ElapsedMilliseconds}");
                licznik++;
                Console.WriteLine($"koniec numer {licznik} czas: {stopWatch.ElapsedMilliseconds}");
            });
            stopWatch.Stop();

            //var times = new StringBuilder();

            //times.AppendLine($"Czas tworzenia tabeli z  danymi pozyskanymi z MongoDB: {stopWatchMakeTable.ElapsedMilliseconds}");
            //times.AppendLine($"Czas tworzenia tabeli zrankingowanych danych: {stopWatchMakeRankingTable.ElapsedMilliseconds}");
            //times.AppendLine($"Czas wykonywania sie algorytmu dla wszystkich zmiennych: {stopWatch.ElapsedMilliseconds}");

            //filtersHelper.GetTimesAndWriteToFile(times, collectionName, "Kendall");

            return corelationArray;
        }


    }

    public class TwoFeatures
    {
        public double PropX { get; set; }
        public double PropY { get; set; }
        public TwoFeatures(double propX, double propY)
        {
            PropX = propX;
            PropY = propY;
        }
    }
}
