using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Display(Name = "Sales Person")]
        public string SalesPersonId { get; set; }

        [ForeignKey("SalesPersonId")]
        public virtual ApplicationUser SalesPerson { get; set; }

        [Required]
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }

        [NotMapped]
        [Required]
        [Display(Name = "Appointment Time")]
        public DateTime AppointmentTime { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string CustomerPhoneNumber { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        public bool IsConfirmed { get; set; }
    }
}
