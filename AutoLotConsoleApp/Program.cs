﻿using AutoLotConsoleApp.EF;
using AutoLotConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
            //FunWithLinqQueries();
            //FindCar5();
            //ChainingLinqQueries();
            //GetAllCarOrdersLazy();
            //GetAllCarOrdersEager();
            GetAllCarOrdersExplicit();
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
        private static void FindCar5()
        {
            using (var context = new AutoLotEntities())
            {
                WriteLine(context.Cars.Find(5));
            }
        }
        private static void ChainingLinqQueries()
        {
            using (var context = new AutoLotEntities())
            {
                // Not executed
                DbSet<Car> allData = context.Cars;

                // Not executed.
                var colorsMakes = from item in allData select new { item.Color, item.Make };

                // Now it's executed
                foreach (var item in colorsMakes)
                {
                    WriteLine(item);
                }
            }
        }
        private static void GetAllCarOrdersLazy()
        {
            // This code is not performant: it initially loads proxies for the order class' then calls a separate sql query for each car's order element.
            using (var context = new AutoLotEntities())
            {
                foreach (Car c in context.Cars)
                {
                    foreach (Order o in c.Orders)
                    {
                        WriteLine(o.OrderId);
                    }
                }
            }
        }
        private static void GetAllCarOrdersEager()
        {
            // An eager loading version of GetAllCarOrdersLazy. Instead of loading proxies for linked Order data, it loads the Order data when it gets the car classes.
            using (var context = new AutoLotEntities())
            {
                foreach (Car c in context.Cars.Include(c => c.Orders))
                {
                    foreach (Order o in c.Orders)
                    {
                        WriteLine(o.OrderId);
                    }
                }
            }
        }
        private static void GetAllCarOrdersExplicit()
        {
            using (var context = new AutoLotEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                foreach (Car c in context.Cars)
                {
                    context.Entry(c).Collection(x => x.Orders).Load();
                    foreach (Order o in c.Orders)
                    {
                        WriteLine(o.OrderId);
                    }
                }
                foreach (Order o in context.Orders)
                {
                    context.Entry(o).Reference(x => x.Car).Load();
                }
            }
        }
        private static void RemoveRecord(int carId)
        {
            // Find a car to delte by primary key
            using (var context = new AutoLotEntities())
            {
                Car carToDelete = context.Cars.Find(carId);
                if (carToDelete != null)
                {
                    context.Cars.Remove(carToDelete);
                    // This code is purely demonstrative to show the entity state changed to Deleted
                    if (context.Entry(carToDelete).State != EntityState.Deleted)
                    {
                        throw new Exception("Unable to delete the record");
                    }
                    context.SaveChanges();
                }
            }
        }
        private static void RemoveMultipleRecords(IEnumerable<Car> carsToRemove)
        {
            using (var context = new AutoLotEntities())
            {
                context.Cars.RemoveRange(carsToRemove);
                context.SaveChanges();
            }
        }
        // Entity state is a more performant, but less secure way to change objects (since it doesn't involve a separate db call to ensure the object to be modified exists.
        private static void RemoveRecordsUsingEntityState(int carId)
        {
            using (var context = new AutoLotEntities())
            {
                Car carToDelete = new Car() { CarId = carId };
                context.Entry(carToDelete).State = EntityState.Deleted;
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    WriteLine(ex);
                }
            }
        }
        private static void UpdateRecord(int carId)
        {
            // Find a car to delete by primary key.
            using (var context = new AutoLotEntities())
            {
                // Grab the car, change it, save.
                Car carToUpdate = context.Cars.Find(carId);
                if (carToUpdate != null)
                {
                    WriteLine(context.Entry(carToUpdate).State);
                    carToUpdate.Color = "Blue";
                    WriteLine(context.Entry(carToUpdate).State);
                    context.SaveChanges();
                }
            }
        }
    }
}
