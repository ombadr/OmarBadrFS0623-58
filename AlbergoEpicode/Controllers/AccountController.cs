using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using AlbergoEpicode.Models;
namespace AlbergoEpicode.Controllers
{
   
    public class AccountController : Controller
    {
        private readonly string _connectionString;

        public AccountController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ConnessioneDBLocale"].ConnectionString;
        }

        private bool AuthenticateUser(string username, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT Password FROM Utenti WHERE Username = @Username";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPasswordHash = reader["Password"].ToString();
                                return BCrypt.Net.BCrypt.Verify(password, storedPasswordHash);
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Username, Password")] LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                bool isValidUser = AuthenticateUser(loginModel.Username, loginModel.Password);
                if (isValidUser)
                {
                    FormsAuthentication.SetAuthCookie(loginModel.Username, false);
                    return RedirectToAction("Index", "Home");
                } else
                {
                    ModelState.AddModelError("Username", "Username o password non validi.");
                    ModelState.AddModelError("Password", "Username o password non validi.");
                    return View();
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Username,Password,ConfirmPassword,Email")] RegisterViewModel registerModel)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO Utenti(Username, Password, Email, RuoloId) VALUES (@Username, @Password, @Email, @RuoloId)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Username", registerModel.Username);
                            command.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(registerModel.Password));
                            command.Parameters.AddWithValue("@Email", registerModel.Email);
                            command.Parameters.AddWithValue("@RuoloId", 2);
                            command.ExecuteNonQuery();
                        }

                        return RedirectToAction("Index", "Home");
                    }

                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError("", "Si è verificato un errore durante la registrazione.");
                }
            }
            return View(registerModel);
        }
    }
}