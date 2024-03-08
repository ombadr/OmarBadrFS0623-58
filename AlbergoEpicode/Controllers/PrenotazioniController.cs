using AlbergoEpicode.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace AlbergoEpicode.Controllers
{
    [Authorize]
    public class PrenotazioniController : Controller
    {
        private readonly string _connectionString;

        public PrenotazioniController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ConnessioneDBLocale"].ConnectionString;
        }

        

        // GET: Prenotazioni
        public ActionResult Index()
        {
            var prenotazioni = new List<Prenotazione>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, CodiceFiscaleCliente, NumeroCamera, DataPrenotazione, NumeroProgressivo, Anno, DataInizio, DataFine, CaparraConfermatoria, TariffaApplicata, DettagliSoggiorno FROM Prenotazioni";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var prenotazione = new Prenotazione
                                {
                                    Id = (int)reader["Id"],
                                    CodiceFiscaleCliente = reader["CodiceFiscaleCliente"].ToString(),
                                    NumeroCamera = (int)reader["NumeroCamera"],
                                    DataPrenotazione = (DateTime)reader["DataPrenotazione"],
                                    NumeroProgressivo = (int)reader["NumeroProgressivo"],
                                    Anno = (int)reader["Anno"],
                                    DataInizio = (DateTime)reader["DataInizio"],
                                    DataFine = (DateTime)reader["DataFine"],
                                    CaparraConfermatoria = (decimal)reader["CaparraConfermatoria"],
                                    TariffaApplicata = (decimal)reader["TariffaApplicata"],
                                    DettagliSoggiorno = reader["DettagliSoggiorno"].ToString()
                                };
                                prenotazioni.Add(prenotazione);
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(prenotazioni);
        }

        public ActionResult Aggiungi()
        {
            List<SelectListItem> camereDisponibili = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT Numero, Descrizione, Tipologia FROM Camere";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    camereDisponibili.Add(new SelectListItem
                    {
                        Text = $"{reader["Descrizione"]} ({reader["Tipologia"]})",
                        Value = reader["Numero"].ToString()
                    });
                }
            }
            ViewBag.CamereDisponibili = camereDisponibili;
            return View();
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aggiungi([Bind(Include = "CodiceFiscaleCliente,NumeroCamera,DataPrenotazione,NumeroProgressivo,Anno,DataInizio,DataFine,CaparraConfermatoria,TariffaApplicata,DettagliSoggiorno")]Prenotazione prenotazione)
        {
            try
            {
                if(ModelState.IsValid)   
                {
                   
                    prenotazione.DataPrenotazione = DateTime.Now;
                   
                    prenotazione.Anno = DateTime.Now.Year;
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        string queryProgressivo = "SELECT ISNULL(MAX(NumeroProgressivo), 0) + 1 FROM Prenotazioni WHERE Anno = @Anno";
                        SqlCommand cmdProgressivo = new SqlCommand(queryProgressivo, connection);
                        cmdProgressivo.Parameters.AddWithValue("@Anno", prenotazione.Anno);
                        int numeroProgressivo = (int)cmdProgressivo.ExecuteScalar();
                        string query = "INSERT INTO Prenotazioni (CodiceFiscaleCliente, NumeroCamera, DataPrenotazione, NumeroProgressivo, Anno, DataInizio, DataFine, CaparraConfermatoria, TariffaApplicata, DettagliSoggiorno) VALUES (@CodiceFiscaleCliente, @NumeroCamera, @DataPrenotazione, @NumeroProgressivo, @Anno, @DataInizio, @DataFine, @CaparraConfermatoria, @TariffaApplicata, @DettagliSoggiorno)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@CodiceFiscaleCliente", prenotazione.CodiceFiscaleCliente);
                            command.Parameters.AddWithValue("@NumeroCamera", prenotazione.NumeroCamera);
                            command.Parameters.AddWithValue("@DataPrenotazione", prenotazione.DataPrenotazione);
                            command.Parameters.AddWithValue("@NumeroProgressivo", numeroProgressivo);
                            command.Parameters.AddWithValue("@Anno", prenotazione.Anno);
                            command.Parameters.AddWithValue("@DataInizio", prenotazione.DataInizio);
                            command.Parameters.AddWithValue("@DataFine", prenotazione.DataFine);
                            command.Parameters.AddWithValue("@CaparraConfermatoria", prenotazione.CaparraConfermatoria);
                            command.Parameters.AddWithValue("@TariffaApplicata", prenotazione.TariffaApplicata);
                            command.Parameters.AddWithValue("@DettagliSoggiorno", prenotazione.DettagliSoggiorno);

                            command.ExecuteNonQuery();
                        }
                    }

                    return RedirectToAction("Index");
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "Errore durante la creazione della prenotazione.");
            }
            return View(prenotazione);
        }

        public ActionResult Checkout(int id)
        {
            CheckoutViewModel model = new CheckoutViewModel();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryPrenotazione = @"SELECT NumeroCamera, DataInizio, DataFine, TariffaApplicata, CaparraConfermatoria
                  FROM Prenotazioni WHERE Id = @PrenotazioneId";

                string queryServizi = @"SELECT LS.Descrizione, SA.Quantita, SA.PrezzoTotale
                    FROM ServiziAggiuntivi AS SA
                    JOIN ListaServizi AS LS ON SA.ServizioId = LS.Id
                    WHERE SA.PrenotazioneId = @PrenotazioneId
                    ";
                
                connection.Open();

                // dettagli prenotazione

                SqlCommand cmd = new SqlCommand(queryPrenotazione, connection);

                cmd.Parameters.AddWithValue("@PrenotazioneId", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.NumeroStanza = Convert.ToInt32(reader["NumeroCamera"]);
                        model.DataInizio = Convert.ToDateTime(reader["DataInizio"]);
                        model.DataFine = Convert.ToDateTime(reader["DataFine"]);
                        model.TariffaApplicata = Convert.ToDecimal(reader["TariffaApplicata"]);
                        model.CaparraConfermatoria = Convert.ToDecimal(reader["CaparraConfermatoria"]);
                    }
                }

                cmd = new SqlCommand(queryServizi, connection);
                cmd.Parameters.AddWithValue("@PrenotazioneId", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        model.ServiziAggiuntivi.Add(new ServizioAggiuntivo
                        {
                            Descrizione = reader["Descrizione"].ToString(),
                            Quantita = Convert.ToInt32(reader["Quantita"]),
                            PrezzoTotale = Convert.ToDecimal(reader["PrezzoTotale"])
                        });
                        model.TotaleServiziAggiuntivi += Convert.ToDecimal(reader["PrezzoTotale"]);

                    }
                }
            }
            return View(model);
        }

        public ActionResult RicercaPrenotazione()
        {
            return View();
        }

        public async Task<ActionResult> RicercaPrenotazioneCliente(string codiceFiscale)
        {
            List<dynamic> prenotazioni = new List<dynamic>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Id, NumeroCamera, DataInizio, DataFine, TariffaApplicata FROM Prenotazioni WHERE CodiceFiscaleCliente = @CodiceFiscale";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CodiceFiscale", codiceFiscale);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync() )
                        {
                            prenotazioni.Add(new
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                NumeroCamera = reader.GetInt32(reader.GetOrdinal("NumeroCamera")),
                                DataInizio = reader.GetDateTime(reader.GetOrdinal("DataInizio")).ToString("yyyy-MM-dd"),
                                DataFine = reader.GetDateTime(reader.GetOrdinal("DataFine")).ToString("yyyy-MM-dd"),
                                TariffaApplicata = reader.GetDecimal(reader.GetOrdinal("TariffaApplicata"))
                            });
                        }
                    }
                }
            }
            return Json(prenotazioni, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrenotazioniPensioneCompleta()
        { return View(); }

        public async Task<ActionResult> ConteggioPensioneCompleta()
        {
            int conteggio = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT COUNT(*) FROM Prenotazioni WHERE DettagliSoggiorno = 'Pensione completa'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    conteggio = (int)await command.ExecuteScalarAsync();
                }
            }
            return Json(new { TotalePensioneCompleta = conteggio }, JsonRequestBehavior.AllowGet);
        }
    }
}