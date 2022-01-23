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
        public double RawFoodDemand { get; set; }
        public double RawFoodSupply { get; set; }
        public double RawFoodTraded { get; set; }
        public double RawFoodPrice { get; set; }
        public double FoodDemand { get; set; }
        public double FoodSupply { get; set; }
        public double FoodTraded { get; set; }
        public double FoodPrice { get; set; }
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
                        200,
                        new List<Recipe>
                        {
                            Constants.Recipes[Constants.RawFood],
                        }, 
                        1.0
                    ),
                    Industry.Create(
                        100,
                        new List<Recipe>
                        {
                            Constants.Recipes[Constants.Food],
                        }, 
                        1.0
                    ),
                },
                Persons = Enumerable.Repeat(0, 100)
                    .Select(_ => new Person())
                    .ToList(),
            };

            var records = new List<CsvRecord>();

            for (var i=-100; i<5000; i++)
            {
                province.Step();

                if (0 <= i && i < 2000 && i%10 == 0)
                {
                    var persons = province.Persons
                        .OrderByDescending(x => x.Inventory.Get(Constants.Money))
                        .Take(10);
                    foreach (var person in persons)
                    {
                        person.Inventory.Remove(Constants.Money, 10);
                    }
                        

                    province.Persons.Add(new Person());
                }

                var unskilledWork = province.Market.History.GetValueOrDefault(Constants.UnskilledWork);
                var rawFood = province.Market.History.GetValueOrDefault(Constants.RawFood);
                var food = province.Market.History.GetValueOrDefault(Constants.Food);

                Console.WriteLine(
                    "{0,5} {1,5:N0} || {2} || {3} || {4}",
                    i,
                    province.Persons.Count,
                    string.Format("{0,6:N0} {1,6:F2}", unskilledWork.AmountTraded, unskilledWork.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", rawFood.AmountTraded, rawFood.AveragePrice),
                    string.Format("{0,6:N0} {1,6:F2}", food.AmountTraded, food.AveragePrice)
                );

                if (0 <= i)
                {
                    records.Add(new CsvRecord
                    {
                        WorkDemand = unskilledWork.AmountToBuy,
                        WorkSupply = unskilledWork.AmountToSell,
                        WorkTraded = unskilledWork.AmountTraded,
                        WorkPrice = unskilledWork.AveragePrice,
                        RawFoodDemand = rawFood.AmountToBuy,
                        RawFoodSupply = rawFood.AmountToSell,
                        RawFoodTraded = rawFood.AmountTraded,
                        RawFoodPrice = rawFood.AveragePrice,
                        FoodDemand = food.AmountToBuy,
                        FoodSupply = food.AmountToSell,
                        FoodTraded = food.AmountTraded,
                        FoodPrice = food.AveragePrice,
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
