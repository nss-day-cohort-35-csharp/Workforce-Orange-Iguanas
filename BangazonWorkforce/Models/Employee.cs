﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int DepartmentId { get; set; }

        public string Email { get; set; }

        public bool isSupervisor { get; set; }

        public int ComputerId { get; set; }

        public Computer Computer { get; set; }

        public Department Department { get; set; }
    }
}