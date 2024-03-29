﻿using System.ComponentModel.DataAnnotations;

namespace PracticeCoreMVC.Models
{
    public class LoginModel
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
