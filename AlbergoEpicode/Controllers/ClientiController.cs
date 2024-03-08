using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using AlbergoEpicode.Models;
using Microsoft.Data.SqlClient;

namespace AlbergoEpicode.Controllers
{
    [Authorize]
    public class ClientiController : Controller
    {
        private readonly string _connectionString;

        public ClientiController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ConnessioneDBLocale"].ConnectionString + ";TrustServerCertificate=True";
        }
        // GET: Clienti
     
        public ActionResult Index()
        {
            var clienti = new List<Cliente>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT CodiceFiscale, Cognome, Nome, Citta, Provincia, Email, Telefono, Cellulare FROM Clienti";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                var cliente = new Cliente
                                {

                                    CodiceFiscale = reader["CodiceFiscale"].ToString(),
                                    Cognome = reader["Cognome"].ToString(),
                                    Nome = reader["Nome"].ToString(),
                                    Citta = reader["Citta"].ToString(),
                                    Provincia = reader["Provincia"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Telefono = reader["Telefono"].ToString(),
                                    Cellulare = reader["Cellulare"].ToString()
                                };
                                clienti.Add(cliente);   
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(clienti);
        }

        public ActionResult Aggiungi()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aggiungi([Bind(Include = "CodiceFiscale,Cognome,Nome,Citta,Provincia,Email,Telefono,Cellulare")]Cliente cliente)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO Clienti (CodiceFiscale, Cognome, Nome, Citta, Provincia, Email, Telefono, Cellulare) VALUES (@CodiceFiscale, @Cognome, @Nome, @Citta, @Provincia, @Email, @Telefono, @Cellulare )";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@CodiceFiscale", cliente.CodiceFiscale);
                            command.Parameters.AddWithValue("@Cognome", cliente.Cognome);
                            command.Parameters.AddWithValue("@Nome", cliente.Nome);
                            command.Parameters.AddWithValue("@Citta", cliente.Citta);
                            command.Parameters.AddWithValue("@Provincia", cliente.Provincia);
                            command.Parameters.AddWithValue("@Email", cliente.Email);
                            command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                            command.Parameters.AddWithValue("@Cellulare", cliente.Cellulare);

                            command.ExecuteNonQuery();
                        }

                        return RedirectToAction("Index");
                    }
                } catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError("", "Impossibile salvare i dati. Riprova, e se il problema persiste, contatta l'amministratore del sistema.");
                }
            }
            return View(cliente);
        }
    }
}