﻿using System.ComponentModel.DataAnnotations;

namespace DiplomService.Models.OrganizationFolder
{
    public class OrganizationApplication
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string OrganizationName { get; set; } = "";

        [Required]
        [MaxLength(40)]
        [EmailAddress]
        public string OrganizationEmail { get; set; } = "";

        public string UserEmail { get; set; } = string.Empty;


        [Required]
        [Display(Name = "Принять заявление")]
        public bool ApplicationApproved { get; set; } = false;

        [Required]
        public DateTime DateOfSend { get; set; } = DateTime.Now;

        [Required]
        public bool Checked { get; set; } = false;

        public string? Message { get; set; }

        [Display(Name = "Ответ пользователю:")]
        public string? ResponseMessage { get; set; }
    }
}
