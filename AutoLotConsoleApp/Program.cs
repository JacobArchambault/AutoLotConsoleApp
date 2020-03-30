using AutoLotConsoleApp.EF;
using AutoLotConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace AutoLotConsoleApp
{
    class Program
    {
        static void Main()
        {
            WriteLine("***** Fun with ADO.NET EF *****\n");
            //int carId = AddNewRecord();
            //WriteLine(carId);
            //ReadLine();

            //List<Car> familyCars = new List<Car>()
            //{
            //    {new Car {Make = "Lexus", Color = "Black", CarNickName = "Moms Hot Rod" } },
            //    {new Car {Make = "Dodge", Color = "Grey", CarNickName = "Big 'ol Truck" } },
            //    {new Car {Make = "Toyota", Color = "Blue", CarNickName = "Betty" } }
            //};
            //AddNewRecords(familyCars);
            //PrintAllInventory();
            //PrintBMWs();
            FunWithLinqQueries();
            ReadLine();
        }
        private static int AddNewRecord()
        {
            // Add record to the Inventory table of the AutoLot database.
            using (var context = new AutoLotEntities())
            {
                try
                {
                    // Hard-code data for a new record, for testing.
                    var car = new Car() { Make = "Yugo", Color = "Brown", CarNickName = "Brownie" };
                    context.Cars.Add(car);
                    context.SaveChanges();
                    // On a successful save, EF populates the database generated identity field.
                    return car.CarId;
                }
                catch (Exception ex)
                {
                    WriteLine(ex.InnerException?.Message);
                    return 0;
                }
            }
        }
        private static void AddNewRecords(IEnumerable<Car> carsToAdd)
        {
            using (var context = new AutoLotEntities())
            {
                context.Cars.AddRange(carsToAdd);
                context.SaveChanges();
            }
        }
        private static void PrintAllInventory()
        {
            // Select all items from the inventory table of Autolot, 
            // and print out the data using our custom ToString() of the Car entity class.
            using (var context = new AutoLotEntities())
            {
                foreach (Car c in context.Cars)
                {
                    WriteLine(c);
                }
            }
        }
        private static void PrintShortCarRecord()
        {
            using (var context = new AutoLotEntities())
            {
                foreach (ShortCar c in context.Database.SqlQuery(typeof(ShortCar), "Select CarId, Make from Inventory"))
                {
                    WriteLine(c);
                }
            }

        }
        private static void PrintBMWs()
        {
            // Select all BMWs from the inventory table of Autolot, 
            // and print out the data using our custom ToString() of the Car entity class.
            using (var context = new AutoLotEntities())
            {
                foreach (Car c in context.Cars.Where(c => c.Make == "BMW"))
                {
                    WriteLine(c);
                }
            }
        }
        private static void FunWithLinqQueries()
        {
            using (var context = new AutoLotEntities())
            {
                // Get a projection of new data.
                var colorsMakes = from item in context.Cars select new { item.Color, item.Make };
                foreach (var item in colorsMakes)
                {
                    WriteLine(item);
                }

                // Get only Black cars.
                var blackCars = from item in context.Cars where item.Color == "Black" select item;
                foreach (var item in blackCars)
                {
                    WriteLine(item);
                }
            }
        }
    }
}
