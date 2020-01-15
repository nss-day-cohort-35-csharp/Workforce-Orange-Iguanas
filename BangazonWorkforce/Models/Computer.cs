using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Computer
    {
<<<<<<< HEAD

=======
        public int Id { get; set; }
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
        [Display(Name="Manufacturer")]
        public string Make { get; set; }
        public string Model { get; set; }
>>>>>>> master
    }
}
