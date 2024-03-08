using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlbergoEpicode.Models
{
    public class AggiungiServizioViewModel
    {
        public int PrenotazioneId { get; set; }

        [Required(ErrorMessage = "Seleziona un servizio.")]
        public int ServizioId { get; set; }
        [Required(ErrorMessage = "La quantità deve essere maggiore di zero.")]
        [Range(1, int.MaxValue, ErrorMessage = "La quantità deve essere maggiore di zero.")]
        public int Quantita { get; set; }
        public List<SelectListItem> ListaServizi { get; set; }

        public AggiungiServizioViewModel()
        {
            ListaServizi = new List<SelectListItem>();
        }
    }
}