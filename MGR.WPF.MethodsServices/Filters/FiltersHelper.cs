using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGR.WPF.MethodsServices.Filters
{
    public class FiltersHelper
    {
        public List<double> RankFeature(List<double> feature)
        {
            List<IdNumber> idNumbersList = feature.Select((n, i) => new IdNumber(i + 1, n)).ToList();
            List<NumberRank> numberRanks = idNumbersList.OrderBy(n => n.Number).Select((n, i) => new NumberRank(n.Id,n.Number, i + 1)).ToList();

            int startCount = -1;
            int attCount = 0;
            for (int i = 1; i <= numberRanks.Count; i++)
            {
                if(i != numberRanks.Count && numberRanks[i-1].Number == numberRanks[i].Number)
                {
                    attCount++;
                    if(startCount == -1)
                    {
                        startCount = i - 1;
                    }
                }
                else
                {
                    if(startCount != -1)
                    {
                        double rankSum = 0;
                        for (int j = startCount; j <= startCount + attCount; j++)
                        {
                            rankSum += numberRanks[j].Rank;
                        }
                        double newRank = rankSum / (attCount + 1);
                        for (int j = startCount; j <= startCount + attCount; j++)
                        {
                            numberRanks[j].Rank = newRank;
                        }
                        startCount = -1;
                        attCount = 0;
                    }
                }
            }

            return numberRanks.OrderBy(x=>x.Id).Select(r => r.Rank).ToList();
        }

        public void SelectFeaturesAndWriteToFile(double[,] corelationArray, int featuresToSelect, string fileName, int featuresCount)
        {
            List<IdNumber> corelationAvg = new List<IdNumber>();
            for (int i = 0; i < featuresCount; i++)
            {
                double sum = 0.00;
                for (int j = 0; j < featuresCount; j++)
                {
                    if (i != j)
                    {
                        if (i < j)
                        {
                            sum += corelationArray[i, j];
                        }
                        else
                        {
                            sum += corelationArray[j, i];
                        }
                    }
                }
                corelationAvg.Add(new IdNumber(i, (sum / (featuresCount - 1))));
            }

            var corelationAvgOrdered = corelationAvg.OrderByDescending(x => x.Number).ToList() ;

            var result = new StringBuilder();

            result.AppendLine("Średnie wyniki korelacji dla poszczególnych zmiennych:");
            foreach (var item in corelationAvg)
            {
                result.AppendLine($"Column{item.Id + 1};{item.Number};");
            }
            result.AppendLine("Wyselekcjonowane zmienne i ich srednie korelacji:");
            foreach (var item in corelationAvgOrdered.Take(featuresToSelect))
            {
                result.AppendLine($"Column{item.Id +1};{item.Number};");
            }
            result.AppendLine($"Zostało wyselekcjonowanych {featuresToSelect.ToString()} zmiennych.");

            result.AppendLine($"Najlepiej skorelowane zmienne:");

            var corelationPairsBest = SelectCorelationPairs(corelationArray, featuresCount, true);
            foreach (var item in corelationPairsBest)
            {
                result.AppendLine($"Column{item.Id1};Column{item.Id2};{item.Number}");
            }
            result.AppendLine($"Nasłabiej skorelowane zmienne:");

            var corelationPairsWorst = SelectCorelationPairs(corelationArray, featuresCount, false);
            foreach (var item in corelationPairsWorst)
            {
                result.AppendLine($"Column{item.Id1};Column{item.Id2};{item.Number}");
            }
            File.WriteAllText($"E://cos//wynikiDobreDoMGR//{fileName}_selectionResult_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.txt", result.ToString());
        }

        public void GetTimesAndWriteToFile(StringBuilder times, string collectionName, string methodName)
        {
            File.WriteAllText($"E://cos//wynikiDobreDoMGR//{methodName}_{collectionName}_times_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.txt", times.ToString());
        }

        public List<CorelationPair> SelectCorelationPairs(double[,] corelationArray, int featuresCount, bool best)
        {
            List<CorelationPair> corelationPairs = new List<CorelationPair>();

            for (int i = 0; i < featuresCount; i++)
            {
                for (int j = i + 1; j < featuresCount; j++)
                {
                    corelationPairs.Add(new CorelationPair(i, j, corelationArray[i, j]));
                }
            }
            if (best)
            {
                return corelationPairs.OrderByDescending(x => x.Number).Take(5).ToList();
            }
            else
            {
                return corelationPairs.OrderBy(x => x.Number).Take(5).ToList();
            }
        }
    }

    public class NumberRank
    {
        public int Id { get; set; }
        public double Number { get; set; }
        public double Rank { get; set; }

        public NumberRank(int id, double number, double rank)
        {
            Id = id;
            Number = number;
            Rank = rank;
        }
    }
    public class IdNumber
    {
        public int Id { get; set; }
        public double Number { get; set; }

        public IdNumber(int id, double number)
        {
            Id = id;
            Number = number;
        }
    }

    public class CorelationPair
    {
        public int Id1 { get; set; }
        public int Id2 { get; set; }
        public double Number { get; set; }

        public CorelationPair(int id1, int id2, double number)
        {
            Id1 = id1;
            Id2 = id2;
            Number = number;
        }
    }
}
