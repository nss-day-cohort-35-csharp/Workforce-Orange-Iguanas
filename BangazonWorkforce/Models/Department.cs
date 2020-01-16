using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace BangazonWorkforce.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Budget { get; set; }

        public List<Employee> Employees { get; set; }
        public int EmployeeCount { get; set; }

        public Employee Employee { get; set; }

        
    }
}