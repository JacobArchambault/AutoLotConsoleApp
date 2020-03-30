using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLotConsoleApp.Models
{
    class ShortCar
    {
        public int CarId { get; set; }
        public string Make { get; set; }
        public override string ToString() => $"{Make} with ID {CarId}";
    }
}
