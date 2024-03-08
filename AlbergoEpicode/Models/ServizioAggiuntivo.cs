using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace AlbergoEpicode.Models
{
    public class ServizioAggiuntivo
    {
        [Required(ErrorMessage = "E' obbligatorio selezionare un servizio.")]
        public string Descrizione { get; set; }
        [Required(ErrorMessage = "E' obbligatorio selezionare la quantità.")]
        [Range(1, int.MaxValue, ErrorMessage = "La quantità deve essere almeno 1.")]
        public int Quantita { get; set; }
        public decimal PrezzoTotale { get; set; }
    }
}