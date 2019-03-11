using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models
{
    public class ProductSelectedForAppointment
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
