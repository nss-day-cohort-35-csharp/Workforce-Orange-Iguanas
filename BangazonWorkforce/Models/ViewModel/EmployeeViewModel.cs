using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModel
{
    public class EmployeeViewModel
    {
        public Employee Employee { get; set; }
        public List<SelectListItem> Departments { get; set; }
        public List<SelectListItem> Computers { get; set; }
    }
}
