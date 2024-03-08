using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace AlbergoEpicode.Models
{
    public class Cliente
    {
        [Required(ErrorMessage = "Il codice fiscale è obbligatorio")]
        [StringLength(16, MinimumLength =16, ErrorMessage = "Il codice fiscale deve essere di 16 caratteri")]
        public string CodiceFiscale { get; set; }
        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il cognome non può superare i 50 caratteri")]
        public string Cognome { get; set; }
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il nome non può superare i 50 caratteri")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "La città è obbligatoria")]
        [StringLength(50, ErrorMessage = "La città non può superare i 50 caratteri")]
        public string Citta { get; set; }
        [Required(ErrorMessage = "La provincia è obbligatoria")]
        [StringLength(50, ErrorMessage = "La provincia non può superare i 50 caratteri")]
        public string Provincia { get; set; }
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        [StringLength(100, ErrorMessage = "L'email non può superare i 50 caratteri")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Il telefono è obbligatorio")]
        [StringLength(20, ErrorMessage = "Il telefono non può superare i 20 caratteri")]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "Il cellulare è obbligatorio")]
        [StringLength(20, ErrorMessage = "Il cellulare non può superare i 20 caratteri")]
        public string Cellulare { get; set; }
    }
}