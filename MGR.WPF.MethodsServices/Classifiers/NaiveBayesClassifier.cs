using MGR.WPF.MethodsServices.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGR.WPF.MethodsServices.Classifier
{
    public class NaiveBayesClassifier
    {
        public NaiveBayesClassifier()
        {

        }

        public void GenerateProbabilites(List<List<double>> dataSet, string collectionName)
        {
            var probabilityOfFeatureValues = new List<ProbabilityOfFeatureValue>();

            var classProbabilty = GetClassProbabilty(dataSet);
            for (int i = 1; i < dataSet.Count; i++)
            {

                foreach (var item in dataSet[i].Distinct())
                {
                    var repeatedValueCount = dataSet[i].Count(r => r == item);
                    int positive = 0;
                    int negative = 0;
                    for (int j = 0; j < dataSet[i].Count; j++)
                    {
                        if (dataSet[i][j].Equals(item))
                        {
                            if (classProbabilty.PositivePosition.Contains(j))
                            {
                                positive++;
                            }
                            else
                            {
                                negative++;
                            }
                        }
                    }
                    var positiveProbability = classProbabilty.PositiveCount > 0 ? (double)positive / (double)classProbabilty.PositiveCount : 0.00;
                    var negativeProbability = classProbabilty.NegativeCount > 0 ? (double)negative / (double)classProbabilty.NegativeCount : 0.00;
                    var allProbability = classProbabilty.AllCount > 0 ? (double)repeatedValueCount / (double)classProbabilty.AllCount : 0.00;
                    probabilityOfFeatureValues.Add(new ProbabilityOfFeatureValue(i, item, positiveProbability, negativeProbability, allProbability));
                }
            }

            var csv = new StringBuilder();
            var newLine = string.Empty;
            foreach (var item in probabilityOfFeatureValues)
            {
                newLine = $"{item.FeatureId};{item.PropertyValue};{item.PositiveProbability};{item.NegativeProbability};{item.AllProbability};";
                csv.AppendLine(newLine);
            }
            File.WriteAllText($"E://cos//wynikiDobreDoMGR//Naive_Bayes_propTable_{collectionName}.txt", csv.ToString());

            //var csv2 = new StringBuilder();
            //csv2.AppendLine($"{classProbabilty.PositiveCount};{classProbabilty.NegativeCount};{classProbabilty.AllCount};");
            

            //var result = ClassifyAllTable(dataSet, classProbabilty, probabilityOfFeatureValues);
        }

        public ClassProbabilty GetClassProbabilty(List<List<double>> dataSet)
        {
            var positivePositions = new List<int>();
            var negativePositions = new List<int>();
            for (int i = 0; i < dataSet[0].Count; i++)
            {
                if ((int)dataSet[0][i] == 1)
                {
                    positivePositions.Add(i);
                }
                else
                {
                    negativePositions.Add(i);
                }
            }

            return new ClassProbabilty(dataSet[0].Count(p => (int)p == 1), dataSet[0].Count(n => (int)n == -1), dataSet[0].Count, positivePositions, negativePositions);
        }

        public bool Classify(List<IdNumber> idNumbersList, ClassProbabilty classProbabilty, List<ProbabilityOfFeatureValue> probabilityOfFeatureValues)
        {
            var pP = 1.00;
            var pN = 1.00;
            var pX = 1.00;
            foreach (var item in idNumbersList)
            {
                var result = probabilityOfFeatureValues.Where(p => p.FeatureId == item.Id && p.PropertyValue.Equals(item.Number)).FirstOrDefault();
                if (result != null)
                {
                    pP *= result.PositiveProbability;
                    pN *= result.NegativeProbability;
                    pX *= result.AllProbability;
                }
            }
            var pPResult = pP / pX;
            var pNResult = pN / pX;
            if(pPResult> pNResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<bool> ClassifyAllTable(List<List<double>> dataSet, ClassProbabilty classProbabilty, List<ProbabilityOfFeatureValue> probabilityOfFeatureValues)
        {
            List<bool> classifyResult = new List<bool>();
            for (int i = 0; i < dataSet[0].Count; i++)
            {
                List<IdNumber> idNumbersList = new List<IdNumber>();
                for (int j = 0; j < dataSet.Count; j++)
                {
                    idNumbersList.Add(new IdNumber(j, dataSet[j][i]));
                }
                classifyResult.Add(Classify(idNumbersList, classProbabilty, probabilityOfFeatureValues));
                Console.WriteLine($"numer klasyfikacji: {i}");
            }
            return classifyResult;
        }

        public List<bool> ClassifyTableWithSelectedFeatures(List<List<double>> dataSet, ClassProbabilty classProbabilty, List<ProbabilityOfFeatureValue> probabilityOfFeatureValues, List<IdNumber> selectedFeaturesWithCorrelation)
        {
            List<bool> classifyResult = new List<bool>();
            
            for (int i = 0; i < dataSet[0].Count; i++)
            {
                List<IdNumber> idNumbersList = new List<IdNumber>();
                foreach (var item in selectedFeaturesWithCorrelation)
                {
                    idNumbersList.Add(new IdNumber(item.Id, dataSet[item.Id][i]));
                }
                classifyResult.Add(Classify(idNumbersList, classProbabilty, probabilityOfFeatureValues));
                Console.WriteLine($"numer klasyfikacji: {i}");
            }
            return classifyResult;
        }


        public List<ProbabilityOfFeatureValue> GetProbabilityOfFeatureValuesFormFile(string collectionName)
        {
            Console.WriteLine("Czytanie danych z pliku");
            using (var reader = new StreamReader($"E://cos//wynikiDobreDoMGR//Naive_Bayes_propTable_{collectionName}.txt"))
            {
                List<ProbabilityOfFeatureValue> probabilityOfFeatureValues = new List<ProbabilityOfFeatureValue>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    probabilityOfFeatureValues.Add(new ProbabilityOfFeatureValue(int.Parse(values[0]), double.Parse(values[1]), double.Parse(values[2]), double.Parse(values[3]), double.Parse(values[4])));
                }
                Console.WriteLine("Czytanie danych z pliku koniec");
                return probabilityOfFeatureValues;
            }            
        }
    }

    public class ProbabilityOfFeatureValue
    {
        public int FeatureId { get; set; }
        public double PropertyValue { get; set; }
        public double PositiveProbability { get; set; }
        public double NegativeProbability { get; set; }
        public double AllProbability { get; set; }
        
        public ProbabilityOfFeatureValue(int featureId, double propertyValue, double positiveProbability, double negativeProbability, double allProbability)
        {
            this.FeatureId = featureId;
            this.PropertyValue = propertyValue;
            this.PositiveProbability = positiveProbability;
            this.NegativeProbability = negativeProbability;
            this.AllProbability = allProbability;
        }
    }

    public class ClassProbabilty
    {
        public int PositiveCount { get; set; }
        public int NegativeCount { get; set; }

        public int AllCount { get; set; }
        public List<int> PositivePosition { get; set; }
        public List<int> NegativePosition { get; set; }
        public ClassProbabilty(int positiveCount, int negativeCount, int allCount, List<int> positivePosition, List<int> negativePosition)
        {
            this.PositiveCount = positiveCount;
            this.NegativeCount = negativeCount;
            this.AllCount = allCount;
            this.PositivePosition = positivePosition;
            this.NegativePosition = negativePosition;
        }
    }
}
