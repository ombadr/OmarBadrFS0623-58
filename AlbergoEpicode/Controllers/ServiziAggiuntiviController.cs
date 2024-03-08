using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlbergoEpicode.Models;

namespace AlbergoEpicode.Controllers
{
    [Authorize]
    public class ServiziAggiuntiviController : Controller
    {
        private readonly string _connectionString;

        public ServiziAggiuntiviController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ConnessioneDBLocale"].ConnectionString;
        }
        // GET: ServiziAggiuntivi
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Aggiungi(int id)
        {
            var model = new AggiungiServizioViewModel
            {
                PrenotazioneId = id,
                ListaServizi = GetListaServizi()
            };
            
            return View(model);  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aggiungi([Bind(Include = "PrenotazioneId,ServizioId,Quantita")]AggiungiServizioViewModel model)

        {
            if (model.Quantita <= 0)
            {
                ModelState.AddModelError("quantita", "La quantità deve essere maggiore di zero.");
            }

            if (model.ServizioId <= 0)
            {
                ModelState.AddModelError("servizioId", "Seleziona un servizio valido.");
            }

            if(ModelState.IsValid)
            {
                decimal prezzo = 0;
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();  
                    string queryPrezzo = "SELECT Prezzo FROM ListaServizi WHERE Id = @ServizioId";
                    SqlCommand commandPrezzo = new SqlCommand(queryPrezzo, connection);
                    commandPrezzo.Parameters.AddWithValue("@ServizioId", model.ServizioId);
                    prezzo = (decimal)commandPrezzo.ExecuteScalar();
                }

                decimal costoTotale = prezzo * model.Quantita;

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string queryInserimento = "INSERT INTO ServiziAggiuntivi (PrenotazioneId, ServizioId, Data, Quantita, PrezzoTotale) VALUES (@PrenotazioneId, @ServizioId, @Data, @Quantita, @PrezzoTotale)";
                    SqlCommand commmandInserimento = new SqlCommand(queryInserimento, connection);

                    commmandInserimento.Parameters.AddWithValue("@PrenotazioneId", model.PrenotazioneId);
                    commmandInserimento.Parameters.AddWithValue("@ServizioId", model.ServizioId);
                    commmandInserimento.Parameters.AddWithValue("@Data", DateTime.Now);
                    commmandInserimento.Parameters.AddWithValue("@Quantita", model.Quantita);
                    commmandInserimento.Parameters.AddWithValue("@PrezzoTotale", costoTotale);

                    commmandInserimento.ExecuteNonQuery();
                }
                return RedirectToAction("Index", "Prenotazioni");
            }

            ViewBag.ListaServizi = GetListaServizi();
            ViewBag.PrenotazioneId = model.PrenotazioneId;
            return View();
        }

        private List<SelectListItem> GetListaServizi()
        {
            var listaServizi = new List<SelectListItem>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Descrizione, Prezzo FROM ListaServizi";
                SqlCommand command = new SqlCommand(query, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaServizi.Add(new SelectListItem
                        {
                            Value = reader["Id"].ToString(),
                            Text = $"{reader["Descrizione"].ToString()} - Prezzo: {reader["Prezzo"].ToString()}€"
                        });
                    }
                }
            }
            return listaServizi;
        }
    }
}