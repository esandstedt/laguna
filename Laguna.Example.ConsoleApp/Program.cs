using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Laguna.Example.ConsoleApp
{
    public class CsvRecord
    {
        public double WorkDemand { get; set; }
        public double WorkSupply { get; set; }
        public double WorkTraded { get; set; }
        public double WorkPrice { get; set; }
        public double FoodDemand { get; set; }
        public double FoodSupply { get; set; }
        public double FoodTraded { get; set; }
        public double FoodPrice { get; set; }
        public double WoodDemand { get; set; }
        public double WoodSupply { get; set; }
        public double WoodTraded { get; set; }
        public double WoodPrice { get; set; }
        public double TimberDemand { get; set; }
        public double TimberSupply { get; set; }
        public double TimberTraded { get; set; }
        public double TimberPrice { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var province = new Province()
            {
                Industries = new List<Industry>()
                {
                    Industry.CreateFarm(300),
                    Industry.CreateForest(100),
                    Industry.CreateSawmill(20),
                },
                Persons = Enumerable.Repeat(0, 500)
                    .Select(_ => new Person())
                    .ToList(),
            };

            var records = new List<CsvRecord>();

            for (var i=0; i<1000; i++)
            {

                province.Step();

                var unskilledWork = province.Market.History.GetValueOrDefault(Constants.UnskilledWork);
                var food = province.Market.History.GetValueOrDefault(Constants.Food);
                var wood = province.Market.History.GetValueOrDefault(Constants.Wood);
                var timber = province.Market.History.GetValueOrDefault(Constants.Timber);

                Console.WriteLine(
                    "{0,5} {1,5:N0} || {2} || {3} || {4} || {5}",
                    i,
                    province.Persons.Count,
                    string.Format("{0,6:N0} {1,6:N0} {2,6:F2}", unskilledWork.AmountTraded, unskilledWork.AmountToSell - unskilledWork.AmountToBuy, unskilledWork.AveragePrice),
                    string.Format("{0,6:N0} {1,6:N0} {2,6:F2}", food.AmountTraded, food.AmountToSell - food.AmountToBuy, food.AveragePrice),
                    string.Format("{0,6:N0} {1,6:N0} {2,6:F2}", wood.AmountTraded, wood.AmountToSell - wood.AmountToBuy, wood.AveragePrice),
                    string.Format("{0,6:N0} {1,6:N0} {2,6:F2}", timber?.AmountTraded, timber?.AmountToSell - timber?.AmountToBuy, timber?.AveragePrice)
                );

                records.Add(new CsvRecord
                {
                    WorkDemand = unskilledWork.AmountToBuy,
                    WorkSupply = unskilledWork.AmountToSell,
                    WorkTraded = unskilledWork.AmountTraded,
                    WorkPrice = unskilledWork.AveragePrice,
                    FoodDemand = food.AmountToBuy,
                    FoodSupply = food.AmountToSell,
                    FoodTraded = food.AmountTraded,
                    FoodPrice = food.AveragePrice,
                    WoodDemand = wood.AmountToBuy,
                    WoodSupply = wood.AmountToSell,
                    WoodTraded = wood.AmountTraded,
                    WoodPrice = wood.AveragePrice,
                    TimberDemand = timber.AmountToBuy,
                    TimberSupply = timber.AmountToSell,
                    TimberTraded = timber.AmountTraded,
                    TimberPrice = timber.AveragePrice,
                });
            }

            var industryMoney = province.Industries.Sum(x => x.Inventory.Get(Constants.Money));
            var personMoney = province.Persons.Sum(x => x.Inventory.Get(Constants.Money));

            { }

            using (var writer = new StreamWriter("output.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }

        }
    }
}
