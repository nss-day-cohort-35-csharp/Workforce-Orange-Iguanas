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
        [MinLength(1, ErrorMessage = "Please enter a First name."), MaxLength(20, ErrorMessage = "Characters exceeds 40 characters")]

        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [MinLength(1, ErrorMessage = "Please enter a Last name."), MaxLength(20, ErrorMessage = "Characters exceeds 40 characters")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Department")]

        [Required]
        public int DepartmentId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Supervisor")]

        public bool IsSupervisor { get; set; }
        [Required]
        [Display(Name = "Computer")]

        public int ComputerId { get; set; }
        public Computer Computer { get; set; }
        public Department Department { get; set; }
            public List <TrainingProgram> trainingPrograms { get; set; }
        public TrainingProgram TrainingProgram { get; set; } 
    }
}