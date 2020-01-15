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
<<<<<<< HEAD
        [Required]
        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        [Required]

=======
        public string FirstName { get; set; }
        public string LastName { get; set; }
>>>>>>> feeace6a0da2f31c784266bb60bbafa107eeb42e
>>>>>>> master
        public int DepartmentId { get; set; }
        public string Email { get; set; }
        public bool isSupervisor { get; set; }
        public int ComputerId { get; set; }
<<<<<<< HEAD
        public Department Department { get; set; }




=======
<<<<<<< HEAD



=======
        public int MyProperty { get; set; }
        public Department department { get; set; }
>>>>>>> feeace6a0da2f31c784266bb60bbafa107eeb42e
>>>>>>> master
    }
}