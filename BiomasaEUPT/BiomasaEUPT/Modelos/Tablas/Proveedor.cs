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
    [Table("Proveedores")]
    public class Proveedor
    {
        [Key]
        public int ProveedorId { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(40)]
        [StringLength(40, MinimumLength = 5)]
        [DisplayName("Razón social"), Display(Name = "Razón social")]
        public string RazonSocial { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        [StringLength(10, MinimumLength = 10)]
        [DisplayName("NIF"), Display(Name = "NIF")]
        public string Nif { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(254)]
        [Index(IsUnique = true)]
        [EmailAddress]
        [DisplayName("Email"), Display(Name = "Email")]
        public string Email { get; set; }

        [MaxLength(60)]
        [DisplayName("Observaciones"), Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(60)]
        [DisplayName("Calle"), Display(Name = "Calle")]
        public string Calle { get; set; }

        public int MunicipioId { get; set; }

        public int TipoId { get; set; }

        [ForeignKey("MunicipioId")]
        public virtual Municipio Municipio { get; set; }

        [ForeignKey("TipoId")]
        public virtual TipoProveedor TipoProveedor { get; set; }
    }
}
