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
        public double GrainDemand { get; set; }
        public double GrainSupply { get; set; }
        public double GrainTraded { get; set; }
        public double GrainPrice { get; set; }
        public double BreadDemand { get; set; }
        public double BreadSupply { get; set; }
        public double BreadTraded { get; set; }
        public double BreadPrice { get; set; }
        public double VegetablesDemand { get; set; }
        public double VegetablesSupply { get; set; }
        public double VegetablesTraded { get; set; }
        public double VegetablesPrice { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var province = new Province()
            {
                Industries = new List<Industry>()
                {
                    Industry.Create(
                        100,
                        new List<Recipe>
                        {
                            Constants.Recipes[Constants.Grain],
                            Constants.Recipes[Constants.Vegetables]
                        }, 
                        1.0
                    ),
                    Industry.Create(
                        20,
                        new List<Recipe>
                        {
                            Constants.Recipes[Constants.Bread],
                        }, 
                        1.0
                    ),
                },
                Persons = Enumerable.Repeat(0, 250)
                    .Select(_ => new Person())
                    .ToList(),
            };

            var records = new List<CsvRecord>();

            for (var i=-100; i<1000; i++)
            {
                province.Step();

                var unskilledWork = province.Market.History.GetValueOrDefault(Constants.UnskilledWork);
                var grain = province.Market.History.GetValueOrDefault(Constants.Grain);
                var bread = province.Market.History.GetValueOrDefault(Constants.Bread);
                var vegetables = province.Market.History.GetValueOrDefault(Constants.Vegetables);
                var fish = province.Market.History.GetValueOrDefault(Constants.Fish);
                var wood = province.Market.History.GetValueOrDefault(Constants.Wood);
                var timber = province.Market.History.GetValueOrDefault(Constants.Timber);
                var barrel = province.Market.History.GetValueOrDefault(Constants.Barrel);

                var hops = province.Market.History.GetValueOrDefault(Constants.Hops);
                var beer = province.Market.History.GetValueOrDefault(Constants.Beer);
                var wine = province.Market.History.GetValueOrDefault(Constants.Wine);
                var clothes = province.Market.History.GetValueOrDefault(Constants.Clothes);

                Console.WriteLine(
                    "{0,5} {1,5:N0} || {2} || {3} || {4} || {5}",
                    i,
                    province.Persons.Count,
                    string.Format("{0,6:N0} {1,6:F2}", unskilledWork.AmountTraded, unskilledWork.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", grain.AmountTraded, grain.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", bread.AmountTraded, bread.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", vegetables.AmountTraded, vegetables.AveragePrice)
                );

                if (0 <= i)
                {
                    records.Add(new CsvRecord
                    {
                        WorkDemand = unskilledWork.AmountToBuy,
                        WorkSupply = unskilledWork.AmountToSell,
                        WorkTraded = unskilledWork.AmountTraded,
                        WorkPrice = unskilledWork.AveragePrice,
                        GrainDemand = grain.AmountToBuy,
                        GrainSupply = grain.AmountToSell,
                        GrainTraded = grain.AmountTraded,
                        GrainPrice = grain.AveragePrice,
                        BreadDemand = bread.AmountToBuy,
                        BreadSupply = bread.AmountToSell,
                        BreadTraded = bread.AmountTraded,
                        BreadPrice = bread.AveragePrice,
                        VegetablesDemand = vegetables.AmountToBuy,
                        VegetablesSupply = vegetables.AmountToSell,
                        VegetablesTraded = vegetables.AmountTraded,
                        VegetablesPrice = vegetables.AveragePrice,
                    });
                }
            }

            var industryMoney = province.Industries.Sum(x => x.Inventory.Get(Constants.Money));
            var personMoney = province.Persons.Sum(x => x.Inventory.Get(Constants.Money));

            { }

            var filePath = string.Format(
                "output_{0}.csv",
                DateTime.Now.ToString("yyyyMMdd_HHmmss")
            );

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }

        }
    }
}
