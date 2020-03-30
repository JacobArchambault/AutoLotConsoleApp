using AutoLotConsoleApp.EF;
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
            int carId = AddNewRecord();
            WriteLine(carId);
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
    }
}
