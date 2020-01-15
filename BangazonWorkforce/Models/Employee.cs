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
<<<<<<< HEAD
        public string FirstName { get; set; }
        public string LastName { get; set; }
=======

        public string FirstName { get; set; }
        public string LastName { get; set; }

>>>>>>> 15987e9faf9a343afec7e9290f923c6283a5e6c7
        public int DepartmentId { get; set; }
        public string Email { get; set; }
        public bool isSupervisor { get; set; }
        public int ComputerId { get; set; }
<<<<<<< HEAD
        public int MyProperty { get; set; }
        public Department Department { get; set; }
        public Computer Computer { get; set; }
=======

        public Computer Computer { get; set; }
        public Department Department { get; set; }


>>>>>>> 15987e9faf9a343afec7e9290f923c6283a5e6c7
    }
}