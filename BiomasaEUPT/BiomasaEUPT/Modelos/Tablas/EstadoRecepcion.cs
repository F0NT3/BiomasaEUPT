﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiomasaEUPT.Modelos.Tablas
{
    [Table("EstadosRecepciones")]
    public class EstadoRecepcion
    {
        [Key]
        public int EstadoRecepcionId { get; set; }

        [Required]
        [MinLength(3)]
        [StringLength(10)]
        [DisplayName("Nombre"), Display(Name = "Nombre")]
        public string Nombre { get; set; }

        public virtual List<Recepcion> Recepciones { get; set; }
    }
}
