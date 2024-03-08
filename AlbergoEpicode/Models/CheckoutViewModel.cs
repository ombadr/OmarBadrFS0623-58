using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlbergoEpicode.Models
{
    public class CheckoutViewModel
    {
        public int NumeroStanza { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        public decimal TariffaApplicata { get; set; }
        public decimal CaparraConfermatoria { get; set; }
        public List<ServizioAggiuntivo> ServiziAggiuntivi { get; set; }
        public decimal TotaleServiziAggiuntivi { get; set; }
        public decimal ImportoDaSaldare { get { return TariffaApplicata - CaparraConfermatoria + TotaleServiziAggiuntivi; } }

        public CheckoutViewModel()
        {
            ServiziAggiuntivi = new List<ServizioAggiuntivo>();
        }
    }
}