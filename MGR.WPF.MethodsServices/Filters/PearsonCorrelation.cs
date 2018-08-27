using MGR.WPF.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGR.WPF.MethodsServices.Filters
{
    public class PearsonCorrelation
    {
        public PearsonCorrelation()
        {

        }
        public void CompereTwoFeatures(List<Int32> featureX, List<Int32> featureY)
        {
            DatabaseService databaseService = new DatabaseService();

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
            var wynik = sumUP / (Math.Sqrt(sumDWX) * Math.Sqrt(sumDWY));
        }
            
        
    }
}
