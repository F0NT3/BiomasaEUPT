﻿using BiomasaEUPT.Modelos;
using BiomasaEUPT.Modelos.Tablas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiomasaEUPT.Clases
{
    public class Trazabilidad
    {
        private BiomasaEUPTContext context;
        public Trazabilidad()
        {
            context = new BiomasaEUPTContext();

            // https://msdn.microsoft.com/en-us/library/jj574232(v=vs.113).aspx
            context.Configuration.LazyLoadingEnabled = false;
        }

        public Proveedor MateriaPrima(string codigo)
        {
            var materiaPrima = context.MateriasPrimas
                .Include("Recepcion.Proveedor.TipoProveedor")
                .Include("Recepcion.Proveedor.Municipio.Provincia.Comunidad.Pais")
                .Include("Recepcion.EstadoRecepcion")
                .Include("TipoMateriaPrima")
                .Include("Procedencia")
                .Include("HistorialHuecosRecepciones.HuecoRecepcion.SitioRecepcion")
                .Include("HistorialHuecosRecepciones.ProductosTerminadosComposiciones.ProductoTerminado.TipoProductoTerminado")
                .Include("HistorialHuecosRecepciones.ProductosTerminadosComposiciones.ProductoTerminado.HistorialHuecosAlmacenajes.HuecoAlmacenaje.SitioAlmacenaje")
                .Single(mp => mp.Codigo == codigo);
            var recepcion = materiaPrima.Recepcion;
            recepcion.MateriasPrimas = new List<MateriaPrima>() { materiaPrima };
            var proveedor = recepcion.Proveedor;
            proveedor.Recepciones = new List<Recepcion>() { recepcion };
            return proveedor;
        }

        public Proveedor Recepcion(string numeroAlbaran)
        {
            var recepcion = context.Recepciones
                .Include("EstadoRecepcion")
                .Include("Proveedor.TipoProveedor")
                .Include("Proveedor.Municipio.Provincia.Comunidad.Pais")
                .Include("MateriasPrimas.Procedencia")
                .Include("MateriasPrimas.TipoMateriaPrima")
                .Include("MateriasPrimas.HistorialHuecosRecepciones.HuecoRecepcion.SitioRecepcion")
                .Include("MateriasPrimas.HistorialHuecosRecepciones.ProductosTerminadosComposiciones.ProductoTerminado.TipoProductoTerminado")
                .Include("MateriasPrimas.HistorialHuecosRecepciones.ProductosTerminadosComposiciones.ProductoTerminado.HistorialHuecosAlmacenajes.HuecoAlmacenaje.SitioAlmacenaje")
                .Single(r => r.NumeroAlbaran == numeroAlbaran);
            var proveedor = recepcion.Proveedor;
            proveedor.Recepciones = new List<Recepcion>() { recepcion };
            return proveedor;
        }

        public List<Proveedor> ProductoTerminado(string codigo)
        {
            var productoTerminado = context.ProductosTerminados
                .Include("TipoProductoTerminado")
                .Include("HistorialHuecosAlmacenajes.HuecoAlmacenaje.SitioAlmacenaje")
                .Include("ProductosTerminadosComposiciones.HistorialHuecoRecepcion.ProductosTerminadosComposiciones.HistorialHuecoRecepcion.HuecoRecepcion.SitioRecepcion")
                .Include("ProductosTerminadosComposiciones.HistorialHuecoRecepcion.ProductosTerminadosComposiciones.HistorialHuecoRecepcion.MateriaPrima.TipoMateriaPrima")
                .Include("ProductosTerminadosComposiciones.HistorialHuecoRecepcion.ProductosTerminadosComposiciones.HistorialHuecoRecepcion.MateriaPrima.Procedencia")
                .Include("ProductosTerminadosComposiciones.HistorialHuecoRecepcion.ProductosTerminadosComposiciones.HistorialHuecoRecepcion.MateriaPrima.Recepcion.EstadoRecepcion")
                .Include("ProductosTerminadosComposiciones.HistorialHuecoRecepcion.ProductosTerminadosComposiciones.HistorialHuecoRecepcion.MateriaPrima.Recepcion.Proveedor.TipoProveedor")
                .Include("ProductosTerminadosComposiciones.HistorialHuecoRecepcion.ProductosTerminadosComposiciones.HistorialHuecoRecepcion.MateriaPrima.Recepcion.Proveedor.Municipio.Provincia.Comunidad.Pais")
                .Single(mp => mp.Codigo == codigo);
            var productosTerminadosComposiciones = productoTerminado.ProductosTerminadosComposiciones.Where(ptc => ptc.ProductoTerminado.Codigo == productoTerminado.Codigo).ToList();
            var materiasPrimas = new List<MateriaPrima>();
            foreach (var ptc in productosTerminadosComposiciones)
            {
                ptc.HistorialHuecoRecepcion.ProductosTerminadosComposiciones = productosTerminadosComposiciones.Where(ptc1 => ptc1.HistorialHuecoId == ptc.HistorialHuecoId).ToList();
                if (!materiasPrimas.Contains(ptc.HistorialHuecoRecepcion.MateriaPrima))
                {
                    materiasPrimas.Add(ptc.HistorialHuecoRecepcion.MateriaPrima);
                }
            }
            var recepciones = new List<Recepcion>();
            foreach (var mp in materiasPrimas)
            {
                if (!recepciones.Contains(mp.Recepcion))
                {
                    recepciones.Add(mp.Recepcion);
                }
            }
            var proveedores = new List<Proveedor>();
            foreach (var r in recepciones)
            {
                if (!proveedores.Contains(r.Proveedor))
                {
                    proveedores.Add(r.Proveedor);
                }
            }
            return proveedores;
        }


        public List<Proveedor> ProductoEnvasado(string codigo)
        {
            var productoEnvasado= context.ProductosEnvasados
               .Include("TipoProductoEnvasado")
               .Include("Picking")
               .Include("ProductoEnvasadoComposiciones.HistorialHuecoAlmacenaje.HuecoAlmacenaje.SitioAlmacenaje")
               .Include("ProductoEnvasadoComposiciones.HistorialHuecoAlmacenaje.ProductoTerminado.TipoProductoTerminado")
               .Include("ProductoEnvasadoComposiciones.HistorialHuecoAlmacenaje.ProductoTerminado.OrdenElaboracion")
               .Include("ProductoEnvasadoComposiciones.HistorialHuecoAlmacenaje.ProductoTerminado.OrdenElaboracion.EstadoElaboracion")
               .Single(mp => mp.Codigo == codigo);
            Console.WriteLine("Entro aqui");
            var productosEnvasadosComposiciones = productoEnvasado.ProductoEnvasadoComposiciones.Where(pec => pec.ProductoEnvasado.Codigo == productoEnvasado.Codigo).ToList();
            var materiasPrimas = new List<MateriaPrima>();
            var productosTerminados = new List<ProductoTerminado>();
            var productosEnvasados = new List<ProductoEnvasado>();
            Console.WriteLine("Entro aqui1");
            foreach (var pec in productosEnvasadosComposiciones)
            {
                pec.HistorialHuecoAlmacenaje.ProductosEnvasadosComposiciones = productosEnvasadosComposiciones.Where(pec1 => pec1.HistorialHuecoId == pec.HistorialHuecoId).ToList();
                if (!productosTerminados.Contains(pec.HistorialHuecoAlmacenaje.ProductoTerminado))
                {
                    productosTerminados.Add(pec.HistorialHuecoAlmacenaje.ProductoTerminado);
                }
            }
            Console.WriteLine("Entro aqui2");
            var envasados = new List<OrdenEnvasado>();
            /*foreach (var pe in productosEnvasados)
            {
                Console.WriteLine("OrdenEnvasado " + pe.OrdenEnvasado.Descripcion);
                if (!envasados.Contains(pe.OrdenEnvasado))
                {
                    envasados.Add(pe.OrdenEnvasado);
                }
            }*/
            Console.WriteLine("Entro aqui3");
            var elaboraciones = new List<OrdenElaboracion>();
            foreach (var pt in productosTerminados)
            {
                Console.WriteLine("OrdenElaboracion " + pt.OrdenElaboracion.Descripcion);
                if (!elaboraciones.Contains(pt.OrdenElaboracion))
                {
                    elaboraciones.Add(pt.OrdenElaboracion);
                }
            }
            Console.WriteLine("Entro aqui4");
            var recepciones = new List<Recepcion>();
            foreach (var mp in materiasPrimas)
            {
                Console.WriteLine("Recepcion " + mp.Recepcion.NumeroAlbaran);
                if (!recepciones.Contains(mp.Recepcion))
                {
                    recepciones.Add(mp.Recepcion);
                }
            }
            Console.WriteLine("Entro aqui5");
            var proveedores = new List<Proveedor>();
            foreach (var r in recepciones)
            {
                Console.WriteLine("Proveedor " + r.Proveedor.RazonSocial);
                if (!proveedores.Contains(r.Proveedor))
                {
                    proveedores.Add(r.Proveedor);
                }
            }
            return proveedores;
        }
    }
}
