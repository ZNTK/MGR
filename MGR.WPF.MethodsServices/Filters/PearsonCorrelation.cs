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
            return sumUP / (Math.Sqrt(sumDWX) * Math.Sqrt(sumDWY));
        }
           
        public double[,] MakeCorelationTable(int featuresCount, string collectionName)
        {
            Stopwatch stopWatch1 = new Stopwatch();
            stopWatch1.Start();
            var dataSet = databaseService.ConvertMongoColectionToListOfLists(featuresCount, collectionName);
            stopWatch1.Stop();
            double[,] corelationArray = new double[featuresCount, featuresCount];
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Parallel.ForEach(dataSet, (list, state, index) =>
            {
                for (int i = (int)index + 1; i < featuresCount; i++)
                {
                    corelationArray[(int)index, i] = Math.Abs(CompereTwoFeatures(list, dataSet[i]));
                }
            });
            

            double max = 0.00;
            string jakiXY = "";
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    if(i != j)
                    {
                        if(corelationArray[i,j] > max)
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
