using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class TrainingProgram
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Please enter a classroom name."), MaxLength(40,ErrorMessage = "Name exceeds 40 characters")]
        public string Name { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        [Range(1,double.MaxValue, ErrorMessage = "Please type in a valid number of Employees attending")]
        public int MaxAttendees { get; set; }
        public List<Employee> Attendees { get; set; }

    }
}
