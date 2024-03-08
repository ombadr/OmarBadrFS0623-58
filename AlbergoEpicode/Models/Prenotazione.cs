using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace AlbergoEpicode.Models
{
    public class Prenotazione
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Il codice fiscale del cliente è obbligatorio.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Il codice fiscale deve essere di 16 caratteri.")]
        public string CodiceFiscaleCliente { get; set; }

        [Required(ErrorMessage = "Il numero della camera è obbligatorio.")]
        public int NumeroCamera { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "La data di prenotazione è obbligatorio.")]
        public DateTime DataPrenotazione { get; set; }
        public int NumeroProgressivo { get; set; }
        public int Anno { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "La data di inizio soggiorno è obbligatoria.")]
        public DateTime DataInizio { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "La data di fine soggiorno è obbligatoria.")]
        public DateTime DataFine { get; set; }

      
        [Required(ErrorMessage = "La caparra confermatoria è obbligatoria.")]
        [Range(0, double.MaxValue, ErrorMessage = "La caparra confermatoria non può essere neativa.")]
        public decimal CaparraConfermatoria { get; set; }

        [Required(ErrorMessage = "La tariffa applicata è obbligatoria.")]
        [Range(0, double.MaxValue, ErrorMessage = "La tariffa applicata non può essere negativa.")]
        public decimal TariffaApplicata { get; set; }

        [Required(ErrorMessage = "Indicare i dettagli del soggiorno.")]
        public string DettagliSoggiorno { get; set; }
    }
}