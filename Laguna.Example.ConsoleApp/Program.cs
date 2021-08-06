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
        public double FishDemand { get; set; }
        public double FishSupply { get; set; }
        public double FishTraded { get; set; }
        public double FishPrice { get; set; }
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
                    Industry.CreateRaw(75, Constants.Fruit, 4, 0.125),
                    Industry.CreateRaw(50, Constants.Vegetables, 4, 0.125),
                    Industry.CreateRaw(50, Constants.Meat, 4, 0.25),
                    Industry.CreateRaw(50, Constants.Fish, 4, 0.25),
                    Industry.CreateRaw(25, Constants.Grain, 4, 0.125),
                    Industry.CreateRaw(100, Constants.Wood, 8, 0.05),

                    //Industry.CreateRaw(50, Constants.Fish, 16, 0.25),
                    //Industry.CreateRaw(25, Constants.Grain, 16, 0.125),

                    Industry.CreateSawmill(20),
                    Industry.CreateBakery(20),
                },
                Persons = Enumerable.Repeat(0, 500)
                    .Select(_ => new Person())
                    .ToList(),
            };

            var records = new List<CsvRecord>();

            for (var i=0; i<1200; i++)
            {
                if (i == 200)
                {
                    province.Industries.Add(Industry.CreateRaw(50, Constants.Fish, 16, 0.25));
                }
                else if (i == 400)
                {
                    province.Industries.Add(Industry.CreateRaw(25, Constants.Grain, 16, 0.125));
                } 
                else if (i == 600) 
                {
                    province.Industries.Add(Industry.CreateBakery(20));
                } 

                province.Step();

                var unskilledWork = province.Market.History.GetValueOrDefault(Constants.UnskilledWork);
                var grain = province.Market.History.GetValueOrDefault(Constants.Grain);
                var bread = province.Market.History.GetValueOrDefault(Constants.Bread);
                var fish = province.Market.History.GetValueOrDefault(Constants.Fish);
                var wood = province.Market.History.GetValueOrDefault(Constants.Wood);
                var timber = province.Market.History.GetValueOrDefault(Constants.Timber);

                Console.WriteLine(
                    "{0,5} {1,5:N0} || {2} || {3} || {4} || {5} || {6} || {7}",
                    i,
                    province.Persons.Count,
                    string.Format("{0,6:N0} {1,6:F2}", unskilledWork.AmountTraded, unskilledWork.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", grain.AmountTraded, grain.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", bread.AmountTraded, bread.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", fish.AmountTraded, fish.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", wood.AmountTraded, wood.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", timber?.AmountTraded, timber?.AveragePrice)
                );

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
                    FishDemand = fish.AmountToBuy,
                    FishSupply = fish.AmountToSell,
                    FishTraded = fish.AmountTraded,
                    FishPrice = fish.AveragePrice,
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
