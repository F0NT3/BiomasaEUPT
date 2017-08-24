﻿using BiomasaEUPT.Clases;
using BiomasaEUPT.Modelos.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BiomasaEUPT.Vistas.GestionElaboraciones
{
    /// <summary>
    /// Lógica de interacción para TablaProductosTerminados.xaml
    /// </summary>
    public partial class TablaProductosTerminados : UserControl
    {
        private Trazabilidad trazabilidad;

        public TablaProductosTerminados()
        {
            InitializeComponent();
            trazabilidad = new Trazabilidad();
        }

        private void bPdfProducto_Click(object sender, RoutedEventArgs e)
        {
            ProductoTerminado productoTerminado = (sender as Button).DataContext as ProductoTerminado;

            InformePDF informe = new InformePDF(Properties.Settings.Default.DirectorioInformes);
            System.Diagnostics.Process.Start(informe.GenerarPDFProductoTerminado(trazabilidad.ProductoTerminado(productoTerminado.Codigo)));
        }

        private void bCodigo_Click(object sender, RoutedEventArgs e)
        {
            ProductoTerminado productoTerminado = (sender as Button).DataContext as ProductoTerminado;

            System.Diagnostics.Process.Start(new InformePDF().GenerarPDFCodigoProductoTerminado(productoTerminado));
        }
    }
}
