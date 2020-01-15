using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class TrainingProgramsController : Controller
    {
        //Config and connection

        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
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

        // GET: TrainingPrograms
        public async Task<ActionResult> Index()
        {

            string sql = @"
                        SELECT t.Id,
                        t.[Name],
                        t.Startdate,
                        t.EndDate,
                        t.MaxAttendees
                        FROM TrainingProgram t
                        WHERE StartDate > @Today";

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new SqlParameter("@Today", DateTime.Today));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<TrainingProgram> programs = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        TrainingProgram program = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                        programs.Add(program);
                    }

                    reader.Close();

                    return View(programs);
                }
            }
        }

        // GET: TrainingPrograms/Details/5
        public ActionResult Details(int id)
        {
            string sql = @"SELECT t.Id,
                    t.[Name],
                    t.StartDate,
                    t.EndDate, 
                    t.MaxAttendees
                    FROM TrainingProgram t
                    WHERE Id = @Id";

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new SqlParameter("@Id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram program = new TrainingProgram();
                    if (reader.Read())
                    {
                        program = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                            Attendees = GetAllEmployees(id)
                        };
                    }

                    reader.Close();

                    return View(program);
                }
            }
        }

        // GET: TrainingPrograms/Create
        public ActionResult Create()
        {
            TrainingProgram program = new TrainingProgram();
            return View(program);
        }

        // POST: TrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TrainingProgram model)
        {
            try
            {
                string sql = @"INSERT INTO TrainingProgram([Name], StartDate, EndDate, MaxAttendees)
                            VALUES(@name, @start, @end, @max)";
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Parameters.Add(new SqlParameter("@name", model.Name));
                        cmd.Parameters.Add(new SqlParameter("@start", model.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@end", model.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@max", model.MaxAttendees));

                        await cmd.ExecuteNonQueryAsync();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingPrograms/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            string sql = @"SELECT t.Id,
                    t.[Name],
                    t.StartDate,
                    t.EndDate, 
                    t.MaxAttendees
                    FROM TrainingProgram t
                    WHERE Id = @Id";

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new SqlParameter("@Id", id));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    TrainingProgram program = new TrainingProgram();
                    if (reader.Read())
                    {
                        program = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                    }

                    reader.Close();

                    return View(program);
                }
            }
        }

        // POST: TrainingPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, TrainingProgram model)
        {
            try
            {
                string sql = @"UPDATE TrainingProgram
                               SET [Name] = @name, StartDate = @start, EndDate = @end, MaxAttendees = @max
                               WHERE Id = @id";
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Parameters.Add(new SqlParameter("@name", model.Name));
                        cmd.Parameters.Add(new SqlParameter("@id", model.Id));
                        cmd.Parameters.Add(new SqlParameter("@start", model.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@end", model.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@max", model.MaxAttendees));
                        await cmd.ExecuteNonQueryAsync();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingPrograms/Delete/5
        public async Task<ActionResult> Delete(int id)
        {

            string sql = @"
                    SELECT t.Id,
                    t.[Name],
                    t.StartDate,
                    t.EndDate,
                    t.MaxAttendees
                    FROM TrainingProgram
                    WHERE t.Id = @Id";

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new SqlParameter("@Id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        TrainingProgram program = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                        reader.Close();

                        return View(program);
                    }
                    else
                    {
                        return NoContent();
                    }
                }
            }
        }

        // POST: TrainingPrograms/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete (int id, TrainingProgram model)
        {
            try
            {
                //Reassign Employees

                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM TrainingProgram WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            catch
            {
                return View();
            }
        }

        //HELPER METHODS----------------------------------

        private List<Employee> GetAllEmployees(int id)
        {
            string sql = @"SELECT e.Id, e.FirstName, e.LastName, e.Email " +
                         "FROM Employee e " +
                         "LEFT JOIN EmployeeTraining as et ON et.EmployeeId = e.Id " +
                         "WHERE et.TrainingProgramId = @trainingid";

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    cmd.Parameters.Add(new SqlParameter("@trainingid", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Email = reader.GetString(reader.GetOrdinal("Email"))
                        });
                    }

                    reader.Close();

                    return employees;
                }
            }
        }

        private async Task<int> ReassignEmployee(int id)
        {
            string sql = @"DELETE FROM EmployeeTraining WHERE TrainingProgramId = @id";

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    return await cmd.ExecuteNonQueryAsync();
                }
            }  
    }
}