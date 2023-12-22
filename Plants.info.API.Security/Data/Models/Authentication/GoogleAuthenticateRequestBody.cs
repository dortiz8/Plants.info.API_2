using System;
using System.ComponentModel.DataAnnotations;

namespace Plants.info.API.Data.Models.Authentication
{
	public class GoogleAuthenticateRequestBody
	{
        [Required]
        public string IdToken { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string Provider { get; set; }
    }
}

