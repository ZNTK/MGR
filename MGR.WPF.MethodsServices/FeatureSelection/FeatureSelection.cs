using MGR.WPF.MethodsServices.Classifier;
using MGR.WPF.MethodsServices.Filters;
using System;
using System.Collections.Generic;
using System.IO;
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

        public List<IdNumber> SelectFeatures(double[,] correlationTable, int featureCount, double coefficientGraterThan)
        {
            Console.WriteLine("wybranie cech");
            var correlationWithClass = new List<IdNumber>();
            for (int i = 0; i < featureCount; i++)
            {
                correlationWithClass.Add(new IdNumber(i, correlationTable[0, i]));
            }
            var avg = correlationWithClass.Average(x => x.Number);
            //var featuresWithGoodCorrelationToClass = correlationWithClass.Where(x => x.Number > avg).ToList();
            var featuresWithGoodCorrelationToClass = correlationWithClass.OrderByDescending(x => x.Number).Take(20).ToList();

            Console.WriteLine("wybranie cech koniec");

            return featuresWithGoodCorrelationToClass;
        }

        public List<IdNumber> SelectFeaturesBetter(double[,] correlationTable, int featureCount, double coefficientGraterThan)
        {
            Console.WriteLine("wybranie cech lepsze");
            var correlationWithClass = new List<IdNumber>();
            for (int i = 0; i < featureCount; i++)
            {
                correlationWithClass.Add(new IdNumber(i, correlationTable[0, i]));
            }
            var avg = correlationWithClass.Average(x => x.Number);
            var featuresWithGoodCorrelationToClass = correlationWithClass.Where(x => x.Number > avg).ToList();

            var correlationWithClassNew = featuresWithGoodCorrelationToClass;
            for (int i = 0; i < featuresWithGoodCorrelationToClass.Count; i++)
            {
                for (int j = i + 1; j < featuresWithGoodCorrelationToClass.Count; j++)
                {
                    if(correlationTable[featuresWithGoodCorrelationToClass[i].Id, featuresWithGoodCorrelationToClass[j].Id] > 0.9
                        && correlationWithClassNew.Contains(featuresWithGoodCorrelationToClass[i]))
                    {
                        correlationWithClassNew.Remove(featuresWithGoodCorrelationToClass[j]);
                    }
                }
                
            }
            Console.WriteLine("wybranie cech lepsze koniec");
            return correlationWithClassNew;
        }

        public void CheckNewModel(List<IdNumber> selectedFeaturesWithCorrelation, List<List<double>> dataSet, List<ProbabilityOfFeatureValue> probabilityOfFeatureValues, string collectionName, bool better, string method)
        {
            NaiveBayesClassifier naiveBayesClassifier = new NaiveBayesClassifier();

            var classProbabilty = naiveBayesClassifier.GetClassProbabilty(dataSet);
            Console.WriteLine("classificationResult start ");
            var classificationResult = naiveBayesClassifier.ClassifyTableWithSelectedFeatures(dataSet, classProbabilty, probabilityOfFeatureValues, selectedFeaturesWithCorrelation);
            Console.WriteLine("classificationResult end ");
            var csv = new StringBuilder();
            var newLine = string.Empty;
            csv.AppendLine("Wynik klasyfikacji:");
            var sumPercent = 0;
            Console.WriteLine("sprawdzanie korelacji");
            List<bool> classToCheck = new List<bool>();
            foreach (var item in dataSet[0])
            {
                if (item.Equals(1.00))
                {
                    classToCheck.Add(true);
                }
                else
                {
                    classToCheck.Add(false);
                }
            }
            for (int i = 0; i < classificationResult.Count; i++)
            {
                newLine = $"{classificationResult[i] == classToCheck[i]} -------> {(classificationResult[i]? 1 : -1)};{dataSet[0][i]}";
                if(classificationResult[i] == classToCheck[i])
                {
                    sumPercent++;
                }
                csv.AppendLine(newLine);
            }

            csv.AppendLine("Zgodność wyników z ograinalnym zbiorem:");
            csv.AppendLine($"{(double)sumPercent/ (double)classificationResult.Count * 100} %");

            csv.AppendLine($"Wybrano {selectedFeaturesWithCorrelation.Count} cechy:");
            foreach (var item in selectedFeaturesWithCorrelation)
            {
                newLine = $"Column{item.Id}";
                csv.AppendLine(newLine);
            }
            File.WriteAllText($"E://cos//wynikiDobreDoMGR//Classification_After_FS_{(better? 2 : 1)}_{collectionName}_{method}.txt", csv.ToString());
        }

        public void SelectFeaturesAndCheckClasification(double[,] correlationTable, int featureCount, List<List<double>> dataSet, string collectionName, string method)
        {
            NaiveBayesClassifier naiveBayesClassifier = new NaiveBayesClassifier();

            var selectedFeatures = SelectFeatures(correlationTable, featureCount, 0.5);
            var probabilityOfFeatureValues = naiveBayesClassifier.GetProbabilityOfFeatureValuesFormFile(collectionName);
            CheckNewModel(selectedFeatures, dataSet, probabilityOfFeatureValues, collectionName,false, method);

            var selectedFeaturesBetter = SelectFeaturesBetter(correlationTable, featureCount, 0.5);
            CheckNewModel(selectedFeaturesBetter, dataSet, probabilityOfFeatureValues, collectionName, true, method);
        }
    }
}
