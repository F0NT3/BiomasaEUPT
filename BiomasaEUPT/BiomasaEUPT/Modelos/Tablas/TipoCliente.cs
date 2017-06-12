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
    /// <summary>
    /// Distintos tipos de clientes con los que la aplicación trabaja
    /// Cada tipo está asociado con un grupo de clientes
    /// </summary>
    [Table("TiposClientes")]
    public class TipoCliente
    {
        [Key]
        public int TipoClienteId { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [StringLength(20, MinimumLength = 3)]
        [Index(IsUnique = true)]
        [DisplayName("Nombre"), Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        [DisplayName("Descripción"), Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        public virtual List<Cliente> Clientes { get; set; }

    }
}
