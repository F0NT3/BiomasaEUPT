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
    [Table("HuecosMateriasPrimas")]
    public class HuecoMateriaPrima
    {
        [Key]
        public int HuecoMateriaPrimaId { get; set; }

        [DisplayName("Volumen"), Display(Name = "Volumen")]
        public double? Volumen { get; set; }

        [Range(0, 1000)]
        [DisplayName("Unidades"), Display(Name = "Unidades")]
        public int? Unidades { get; set; }

        [DisplayName("Volumen utilizado"), Display(Name = "Volumen utilizado")]
        public double? VolumenUtilizado { get; set; }

        [Range(0, 1000)]
        [DisplayName("Unidades utilizadas"), Display(Name = "Unidades utilizadas")]
        public int? UnidadesUtilizadas { get; set; }

        public int MateriaPrimaId { get; set; }

        public int HuecoRecepcionId { get; set; }

        [ForeignKey("MateriaPrimaId")]
        public virtual MateriaPrima MateriaPrima { get; set; }

        [ForeignKey("HuecoRecepcionId")]
        public virtual HuecoRecepcion HuecoRecepcion { get; set; }
    }
}
