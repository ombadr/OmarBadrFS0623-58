using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlbergoEpicode.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Il campo username è obbligatorio.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Il campo password è obbligatorio.")]
        public string Password { get; set; }
    }
}