using DiplomService.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomService.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }


        public byte[]? PriviewImage { get; set; } = null;

        [MaxLength(40)]
        [Display(Name = "Название*")]
        public string Name { get; set; } = "";

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Display(Name = "Дата начала*")]
        public DateTime DateOfStart { get; set; }

        [Display(Name = "Дата окончания")]
        public DateTime? DateOfEnd { get; set; }

        [Display(Name = "Является ли событие публичным*")]
        public bool PublicEvent { get; set; } = true;

        [Required]
        public bool ReadyToShow { get; set; } = false;

        [Required]
        public bool DivisionsExist { get; set; } = true;

        [JsonIgnore]
        public virtual List<Organization> Organizations { get; set; } = new();
        [JsonIgnore]
        public virtual List<Measure> Measures { get; set; } = new();
        [JsonIgnore]
        public virtual List<Division> Divisions { get; set; } = new();
        [JsonIgnore]
        public virtual List<News> News { get; set; } = new();
        [JsonIgnore]
        public virtual List<EventApplication>? EventApplications { get; set; }

        [NotMapped]
        public string? MimeType { get { return ImageWorker.GetImageMimeType(PriviewImage); } }

        [NotMapped]
        public int EventMembers
        {
            get
            {
                return Divisions.SelectMany(d => d.DivisionMembers).Select(x=>x.User).Distinct().Count();
            }
        }
    }
}
