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
            

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Parallel.ForEach(rankDataSet, (rankList, state, index) =>
            {
                Console.WriteLine($"robie numer {index} czas: {stopWatch.ElapsedMilliseconds}");
                for (int i = (int)index + 1; i < featuresCount; i++)
                {
                    //if(index == 11 && i == 13)
                    //{
                    //    var csv = new StringBuilder();
                    //    for (int j = 0; j < rankList.Count; j++)
                    //    {
                    //        csv.AppendLine($"{rankList[j]};{rankDataSet[i][j]};");
                    //    }
                    //    File.WriteAllText($"E://cos//dobrytestczemuduze.txt", csv.ToString());
                    //}
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
