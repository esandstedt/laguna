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
        public double FruitDemand { get; set; }
        public double FruitSupply { get; set; }
        public double FruitTraded { get; set; }
        public double FruitPrice { get; set; }
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
        public double BarrelDemand { get; set; }
        public double BarrelSupply { get; set; }
        public double BarrelTraded { get; set; }
        public double BarrelPrice { get; set; }
        public double WineDemand { get; set; }
        public double WineSupply { get; set; }
        public double WineTraded { get; set; }
        public double WinePrice { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var province = new Province()
            {
                Industries = new List<Industry>()
                {
                    Industry.Create(50, Constants.Recipes[Constants.Fruit], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Fruit], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Vegetables], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Vegetables], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Meat], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Meat], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Fish], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Fish], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Grain], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Wood], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Wood], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Wood], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Wood], 0.25),

                    //Industry.CreateRaw(50, Constants.Fish, 16, 0.25),
                    //Industry.CreateRaw(25, Constants.Grain, 16, 0.125),

                    Industry.Create(25, Constants.Recipes[Constants.Timber], 1.0),
                    Industry.Create(25, Constants.Recipes[Constants.Bread], 1.0),

                    Industry.Create(50, Constants.Recipes[Constants.Hops], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Hops], 0.25),
                    Industry.Create(25, Constants.Recipes[Constants.Beer], 1.0),
                    Industry.Create(25, Constants.Recipes[Constants.Wine], 1.0),
                    Industry.Create(25, Constants.Recipes[Constants.Barrel], 1.0),
                    Industry.Create(25, Constants.Recipes[Constants.Furniture], 1.0),
                    Industry.Create(50, Constants.Recipes[Constants.Cotton], 0.25),
                    Industry.Create(50, Constants.Recipes[Constants.Cotton], 0.25),
                    Industry.Create(25, Constants.Recipes[Constants.Fabric], 1.0),
                    Industry.Create(25, Constants.Recipes[Constants.Clothes], 1.0),
                },
                Persons = Enumerable.Repeat(0, 250)
                    .Select(_ => new Person())
                    .ToList(),
            };

            var records = new List<CsvRecord>();

            for (var i=-100; i<1200; i++)
            {
                if (i%4 == 0 && 0 < i && i <= 1000)
                {
                    province.Persons.Add(new Person());
                }

                if (i == 100)
                {
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Fish], 1.0));
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Fish], 1.0));
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Grain], 1.0));
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Grain], 1.0));
                }
                else if (i == 200)
                {
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Fruit], 1.0));
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Fruit], 1.0));
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Bread], 1.0));
                } 
                else if (i == 300)
                {
                    province.Industries.Add(Industry.Create(25, Constants.Recipes[Constants.Bread], 1.0));
                }
                else if (i == 350)
                {
                    province.Industries.Add(Industry.Create(25, Constants.Recipes[Constants.Bread], 1.0));
                }
                else if (i == 400) 
                {
                    province.Industries.Add(Industry.Create(25, Constants.Recipes[Constants.Bread], 1.0));
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Wood], 1.0));
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Wood], 1.0));
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Wood], 1.0));
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Wood], 1.0));
                } 
                else if (i == 450)
                {
                    province.Industries.Add(Industry.Create(25, Constants.Recipes[Constants.Bread], 1.0));
                }
                else if (i == 500) 
                {
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Timber], 1.0));
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Timber], 1.0));
                } 
                else if (i == 550) 
                {
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Wood], 1.0));
                    province.Industries.Add(Industry.Create(50, Constants.Recipes[Constants.Wood], 1.0));
                } 
                else if (i == 600)
                {
                    province.Industries.Add(Industry.Create(25, Constants.Recipes[Constants.Beer], 1.0));
                    province.Industries.Add(Industry.Create(25, Constants.Recipes[Constants.Wine], 1.0));
                }
                else if (i == 900)
                {
                    var diffs = province.Market.History
                        .Select(x => new
                        {
                            Commodity = x.Key,
                            Diff = x.Value.AmountToBuy - x.Value.AmountToSell
                        })
                        .OrderByDescending(x => x.Diff)
                        .ToList();

                    { }
                }

                province.Step();

                var unskilledWork = province.Market.History.GetValueOrDefault(Constants.UnskilledWork);
                var grain = province.Market.History.GetValueOrDefault(Constants.Grain);
                var bread = province.Market.History.GetValueOrDefault(Constants.Bread);
                var fruit = province.Market.History.GetValueOrDefault(Constants.Fruit);
                var fish = province.Market.History.GetValueOrDefault(Constants.Fish);
                var wood = province.Market.History.GetValueOrDefault(Constants.Wood);
                var timber = province.Market.History.GetValueOrDefault(Constants.Timber);
                var barrel = province.Market.History.GetValueOrDefault(Constants.Barrel);

                var hops = province.Market.History.GetValueOrDefault(Constants.Hops);
                var beer = province.Market.History.GetValueOrDefault(Constants.Beer);
                var wine = province.Market.History.GetValueOrDefault(Constants.Wine);
                var clothes = province.Market.History.GetValueOrDefault(Constants.Clothes);

                Console.WriteLine(
                    "{0,5} {1,5:N0} || {2} || {3} || {4} || {5} || {6} || {7}",
                    i,
                    province.Persons.Count,
                    string.Format("{0,6:N0} {1,6:F2}", unskilledWork.AmountTraded, unskilledWork.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", hops.AmountTraded, beer.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", beer.AmountTraded, beer.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", fruit.AmountTraded, beer.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", wine.AmountTraded, wine.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", clothes.AmountTraded, clothes.AveragePrice)
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
                        FruitDemand = fruit.AmountToBuy,
                        FruitSupply = fruit.AmountToSell,
                        FruitTraded = fruit.AmountTraded,
                        FruitPrice = fruit.AveragePrice,
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
                        BarrelDemand = barrel.AmountToBuy,
                        BarrelSupply = barrel.AmountToSell,
                        BarrelTraded = barrel.AmountTraded,
                        BarrelPrice = barrel.AveragePrice,
                        WineDemand = wine.AmountToBuy,
                        WineSupply = wine.AmountToSell,
                        WineTraded = wine.AmountTraded,
                        WinePrice = wine.AveragePrice,
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
