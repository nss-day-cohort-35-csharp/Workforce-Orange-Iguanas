using System.Xml.Schema;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;
        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Employees
        public ActionResult Index()
        {
            var employees = GetAllEmployees();
            return View(employees);

        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)

        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.IsSupervisor, e.Email, c.Make, c.Model, d.[Name] AS DepartmentName 
                                        FROM Employee e JOIN Department d ON e.DepartmentId = d.Id Join Computer c ON e.ComputerId = c.Id
                                        WHERE e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();
                    Employee employee = null;
                     while(reader.Read())
                    {    
                        if(employee == null ){
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor")),

                            Department = new Department
                            
                            {
                            Name = reader.GetString(reader.GetOrdinal("DepartmentName"))
                            },
                            Computer = new Computer {
                               Make = reader.GetString(reader.GetOrdinal("Make")),
                               Model = reader.GetString(reader.GetOrdinal("Model")) 
                            } 
                        };
                        }
                        employee.trainingPrograms = GetTrainingProgramsByEmployeeId(id);

                        
                        return View(employee);
                    }
                    reader.Close();
                    return NotFound();

                }
            }
            
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            var departments = GetDepartments().Select(d => new SelectListItem
            {
                Text = d.Name,
                    Value = d.Id.ToString()
            }).ToList();

            var computers = GetUnAssignedComputers().Select(c => new SelectListItem
            {
                Text = c.Make + " " + c.Model,
                    Value = c.Id.ToString()
            }).ToList();
            var viewModel = new EmployeeViewModel()
            {
                Employee = new Employee(),
                Departments = departments,

                Computers = computers

            };
            return View(viewModel);

        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            {
                try
                {
                    using(SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using(SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, Email, IsSupervisor, DepartmentId, ComputerId)
                                            VALUES (@FirstName, @LastName, @Email, @IsSupervisor,@DepartmentId,@ComputerId )";

                            cmd.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                            cmd.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
                            cmd.Parameters.Add(new SqlParameter("@Email", employee.Email));
                            cmd.Parameters.Add(new SqlParameter("@IsSupervisor", employee.IsSupervisor));
                            cmd.Parameters.Add(new SqlParameter("@DepartmentId", employee.DepartmentId));
                            cmd.Parameters.Add(new SqlParameter("@ComputerId", employee.ComputerId));

                            cmd.ExecuteNonQuery();
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var computers = GetComputers().Select(c => new SelectListItem
                {
                    Text = c.Make + " " + c.Model,
                        Value = c.Id.ToString()
                }).ToList();
                var departments = GetDepartments().Select(d => new SelectListItem
                {
                    Text = d.Name,
                        Value = d.Id.ToString()
                }).ToList();
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, FirstName, LastName, DepartmentId, Email, ComputerId 
                                        FROM Employee
                                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                ComputerId = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                            };
                            reader.Close();

                            var viewModel = new EmployeeViewModel()
                            {
                                Employee = employee,
                                Departments = departments,
                                Computers = computers
                            };
                            return View(viewModel);
                        };
                        reader.Close();
                        return NotFound();
                    }
                }
            }
            catch (Exception e)
            {
                return View();
            }

        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Employee employee)
        {
            try
            {
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee
                                            Set 
                                            LastName = @lastName, 
                                            DepartmentId = @departmentId,
                                            Email = @email,
                                            ComputerId = @computerId,
                                            isSupervisor = @isSupervisor
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@computerId", employee.ComputerId));
                        cmd.Parameters.Add(new SqlParameter("@email", employee.Email));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private List<Department> GetDepartments()
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name
                                       FROM Department";

                    var reader = cmd.ExecuteReader();

                    var departments = new List<Department>();

                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))

                        });
                    }

                    reader.Close();

                    return departments;
                }
            }
        }

        private List<Computer> GetUnAssignedComputers()
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.PurchaseDate, c.Make, c.Model FROM Computer c 
                                        LEFT JOIN Employee e ON c.Id = e.ComputerId
                                        WHERE e.Id IS NULL AND c.DecomissionDate IS NULL";

                    var reader = cmd.ExecuteReader();

                    var computers = new List<Computer>();

                    while (reader.Read())
                    {
                        computers.Add(new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Model = reader.GetString(reader.GetOrdinal("Model"))

                        });
                    }

                    reader.Close();

                    return computers;
                }
            }
        }

        //private List<Department> GetDepartments()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT Id, Name, Budget 
        //                               FROM Department";

        //            var reader = cmd.ExecuteReader();

        //            var departments = new List<Department>();

        //            while (reader.Read())
        //            {
        //                departments.Add(new Department
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    Name = reader.GetString(reader.GetOrdinal("Name")),
        //                    Budget = reader.GetInt32(reader.GetOrdinal("Budget"))

        //                });
        //            }

        //            reader.Close();

        //            return departments;
        //        }

        //    }
        //}
        private List<Computer> GetComputers()
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Make, Model 
                                       FROM Computer";
                    SqlDataReader reader = cmd.ExecuteReader();
                    var computers = new List<Computer>();
                    while (reader.Read())
                    {

                        computers.Add(new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Model = reader.GetString(reader.GetOrdinal("Model"))
                        });
                    }
                    reader.Close();

                    return computers;
                }
            }
        }
        public ActionResult AssignTraining(int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id, tp.Name, tp.StartDate, tp.EndDate 
                                       FROM TrainingProgram tp WHERE tp.Id NOT IN (SELECT TrainingProgramId FROM EmployeeTraining WHERE EmployeeId = @employeeId)";
                    cmd.Parameters.Add(new SqlParameter("@employeeId", Id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    var trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {

                        trainingPrograms.Add(new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),

                        });
                       
                    }
                    
                    reader.Close();

                    return View(trainingPrograms);
                }
            }
        }
        [HttpPost]
        public ActionResult AssignTraining(int id, List<int> trainingProgramIds)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                foreach( var trainingProgramId in trainingProgramIds)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId)
                                            VALUES(@EmployeeId, @TrainingProgramId)";

                        cmd.Parameters.Add(new SqlParameter("@EmployeeId", id));
                        cmd.Parameters.Add(new SqlParameter("@TrainingProgramId", trainingProgramId));
                        cmd.ExecuteNonQuery();
                    };
                }
            }
            return RedirectToAction("Details", new { id = id });
        }
        private List<Employee> GetAllEmployees()
        {
            // step 1 open the connection
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    // step 2. create the query
                    cmd.CommandText = @"Select e.Id, e.FirstName, e.LastName,  d.[Name] As Department FROM Employee e 

                                    Left Join Department d ON e.DepartmentId = d.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    // create a collection to keep the list of cohorts
                    List<Employee> employees = new List<Employee>();

                    // run the query and hold the results in an object
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {

                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Department = new Department
                            {
                            Name = reader.GetString(reader.GetOrdinal("Department"))
                            }
                        };
                        employees.Add(employee);

                    }
                    reader.Close();
                    return employees;
                }
            }

        }

        private List<TrainingProgram> GetAllTrainingPrograms(int employeeId)
        {

            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    // step 2. create the query
                    cmd.CommandText = @"Select tp.Id, tp.Name, tp.StartDate, tp.EndDate FROM TrainingProgram tp 
                                    Inner Join EmployeeTraining et ON tp.Id = et.TrainingProgramId
                                    WHERE EmployeeId = @employeeId";
                                    cmd.Parameters.Add(new SqlParameter("@employeeId", employeeId));
                    SqlDataReader reader = cmd.ExecuteReader();
                    // create a collection to keep the list of cohorts
                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    // run the query and hold the results in an object
                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {

                            Id = reader.GetInt32(reader.GetOrdinal("Id")),

                        };
                        trainingPrograms.Add(trainingProgram);
                    }
                    reader.Close();
                    return trainingPrograms;
                }
            }
        }
        private List<TrainingProgram> GetTrainingProgramsByEmployeeId(int employeeId)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // step 2. create the query
                    cmd.CommandText = @"Select tp.Id, tp.Name, tp.StartDate, tp.EndDate FROM TrainingProgram tp 
                                    Inner Join EmployeeTraining et ON tp.Id = et.TrainingProgramId
                                    WHERE EmployeeId = @employeeId";
                    cmd.Parameters.Add(new SqlParameter("@employeeId", employeeId));
                    SqlDataReader reader = cmd.ExecuteReader();
                    // create a collection to keep the list of cohorts
                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    // run the query and hold the results in an object
                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {

                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),

                        };
                        trainingPrograms.Add(trainingProgram);
                    }
                    reader.Close();
                    return trainingPrograms;
                }
            }
        }
    }
}

//private List<Department> GetDepartments()
//{
//    using (SqlConnection conn = Connection)
//    {
//        conn.Open();
//        using (SqlCommand cmd = conn.CreateCommand())
//        {
//            cmd.CommandText = @"SELECT Id, Name, Budget 
//                               FROM Departments";

//            var reader = cmd.ExecuteReader();

//            var departments = new List<Department>();

//            while (reader.Read())
//            {
//                departments.Add(new Department
//                {
//                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                    Name = reader.GetString(reader.GetOrdinal("Name")),
//                    Budget = reader.GetInt32(reader.GetOrdinal("Budget"))

//                });
//            }

//            reader.Close();

//            return departments;
//        }
//    }
//}