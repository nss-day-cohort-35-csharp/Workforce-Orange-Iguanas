using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        [Required]

        public int DepartmentId { get; set; }
        public string Email { get; set; }
        public bool isSupervisor { get; set; }
        public int ComputerId { get; set; }



    }
}
