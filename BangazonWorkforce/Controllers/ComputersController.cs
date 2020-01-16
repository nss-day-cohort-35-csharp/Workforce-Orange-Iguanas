using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class ComputersController : Controller

    {
        private readonly IConfiguration _config;
        public ComputersController(IConfiguration config)
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
        // GET: Computers
        public ActionResult Index(string searchString)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, PurchaseDate, Make, Model FROM Computer ";
                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        cmd.CommandText += @" WHERE Make LIKE @searchString OR Model LIKE @searchString";
                    }
                    cmd.Parameters.Add(new SqlParameter("@searchString", "%" + searchString + "%"));
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
                    return View(computers);
                }
            }
        }

        // GET: Computer/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: Computer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Computer computer)
        {
            {
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"INSERT INTO Computer (PurchaseDate, Make, Model)
                                            VALUES (@PurchaseDate, @Make, @Model)";

                            cmd.Parameters.Add(new SqlParameter("@PurchaseDate", computer.PurchaseDate));
                            cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                            cmd.Parameters.Add(new SqlParameter("@Model", computer.Model));
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

        // GET: Computer/Delete/5
        public ActionResult Delete(int id)
        {
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, PurchaseDate, Make, Model FROM Computer
                                        WHERE Id = @Id";

                        cmd.Parameters.Add(new SqlParameter("@Id", id));

                        var reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            var computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Model = reader.GetString(reader.GetOrdinal("Model"))

                            };

                            reader.Close();
                            ViewBag.Employee = GetEmployeeByComputerId(id);
                            return View(computer);
                        }
                        return NotFound();
                    }
                }
            }
        }

        // POST: Computer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Computer computer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Computer WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }

            }
            catch
            {
                return View();
            }
        }

        private Employee GetEmployeeByComputerId(int computerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName FROM Employee
                                        WHERE ComputerId = @ComputerId ";
                    
                    cmd.Parameters.AddWithValue("@ComputerId", computerId);
                    var reader = cmd.ExecuteReader();
                    Employee employee = null;
                    while (reader.Read())
                    {
                       employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))

                        };
                    }
                    reader.Close();
                    return employee;
                }
            }
        }
    }
}