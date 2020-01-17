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
        [Display(Name = "Title")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Starting Date")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "Ending Date")]
        public DateTime EndDate { get; set; }
        [Required]
        [Range(1,double.MaxValue, ErrorMessage = "Please type in a valid number of Employees attending")]
        [Display(Name = "Maximum Attendees")]
        public int MaxAttendees { get; set; }
        public List<Employee> Attendees { get; set; }

        
    }
}
