using System;
using System.Collections.Generic;
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
}
