using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SSO.WCFService.DataContracts
{
    [DataContract]
    public class RegisterRequest
    {
        [DataMember(IsRequired = true)]
//      [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } //TODO: should this be automatically generated or.. ?

        [DataMember(IsRequired = true)]
//      [Required(ErrorMessage = "Email is required.")]
//      [RegularExpression(@"\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b", ErrorMessage = "Invalid mail format.")]

        public string Email { get; set; }

        [DataMember(IsRequired = true)]
//      [Required(ErrorMessage = "Password is required.")]
//      [StringLength(16, MinimumLength = 8, ErrorMessage = "Password length should be between 10 and 100." )]
        public string Password { get; set; }

        [DataMember(IsRequired = true)]
//      [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [DataMember(IsRequired = true)]
//      [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

    }
}