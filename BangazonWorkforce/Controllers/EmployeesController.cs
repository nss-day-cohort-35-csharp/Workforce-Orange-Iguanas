﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using BangazonWorkforce.Models.ViewModel;

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
            return View();
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var departments = GetDepartments().Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString()
            }).ToList();

            var viewModel = new EmployeeViewModel()
            {
                Employee = new Employee(),
                Departments = departments

            };

            return View(viewModel);


        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }   

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {  try
            {

            var departments = GetDepartments().Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString()
            }).ToList();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, DepartmentId 
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
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                        };
                        reader.Close();

                        var viewModel = new EmployeeViewModel
                        {
                            Employee = employee,
                            Departments = departments
                        };
                        return View(viewModel);
                    };
                    reader.Close();
                    return NotFound();
            }
                }
            }catch (Exception e)
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
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee
                                            Set 
                                            LastName = @lastName, 
                                            DepartmentId = @departmentId
                                            WHERE Id = @id";

                       
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
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

        private List<Employee> GetAllEmployees()
        {
            // step 1 open the connection
            using SqlConnection conn = Connection;
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                // step 2. create the query
                cmd.CommandText = @"Select e.Id, e.FirstName, e.LastName,  d.[Name] As Department FROM Employee e 

                                    Left Join Department d ON e.DepartmentId = d.Id";
                                 

                SqlDataReader reader = cmd.ExecuteReader();

                // create a collection to keep the list of cohorts
                List<Employee> employees = new List<Employee>();

                // run the query and hold the results in an object
                while(reader.Read())
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
                
                    //add to the list of cohorts 


                    
                    //close the connection and return the list of cohorts

                    
                }
                    reader.Close();
                return employees; 
                // GET: Departments List
            }
        }
            private List<Department> GetDepartments()
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, Name, Budget 
                                       FROM Department";

                        var reader = cmd.ExecuteReader();

                        var departments = new List<Department>();

                        while (reader.Read())
                        {
                            departments.Add(new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))

                            });
                        }

                        reader.Close();

                        return departments;
                    }

                }
            }
        }
    }
