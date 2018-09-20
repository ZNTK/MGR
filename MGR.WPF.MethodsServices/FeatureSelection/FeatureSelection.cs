using MGR.WPF.MethodsServices.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGR.WPF.MethodsServices.FeatureSelection
{
    public class FeatureSelection
    {
        public FeatureSelection()
        {

        }

        public void SelectFeatures(double[,] correlationTable, int featureCount, double coefficientGraterThan)
        {
            var correlationWithClass = new List<IdNumber>();
            for (int i = 0; i < featureCount; i++)
            {
                correlationWithClass.Add(new IdNumber(i, correlationTable[0, i]));
            }

            var featuresWithGoodCorrelationToClass = correlationWithClass.Where(x => x.Number > coefficientGraterThan).ToList();




        }
    }
}
