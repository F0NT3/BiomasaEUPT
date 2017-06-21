﻿using BiomasaEUPT.Clases;
using BiomasaEUPT.Modelos;
using BiomasaEUPT.Modelos.Tablas;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
    /// Lógica de interacción para TabElaboraciones.xaml
    /// </summary>
    public partial class TabElaboraciones : UserControl
    {
        private BiomasaEUPTContext context;
        private CollectionViewSource ordenesElaboracionesViewSource;
        private CollectionViewSource productosTerminadosViewSource;

        public TabElaboraciones()
        {
            InitializeComponent();
            DataContext = this;

            //ucTablaRecepciones.dgRecepciones.SelectionChanged += DgRecepciones_SelectionChanged;
            //ucTablaRecepciones.cbNumeroAlbaran.Checked += (s, e1) => { FiltrarTablaRecepciones(); };
            ucTablaElaboraciones.dgElaboraciones.SelectionChanged += DgElaboraciones_SelectionChanged;

            ucTablaProductosTerminados.bAnadirProductoTerminado.Click += BAnadirProductoTerminado_Click;

            Style rowStyle = new Style(typeof(DataGridRow), (Style)TryFindResource(typeof(DataGridRow)));
            rowStyle.Setters.Add(new EventSetter(MouseDoubleClickEvent, new MouseButtonEventHandler(RowElaboraciones_DoubleClick)));
            ucTablaElaboraciones.dgElaboraciones.RowStyle = rowStyle;
            ucTablaElaboraciones.dgElaboraciones.SelectedIndex = -1;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            using (new CursorEspera())
            {
                context = new BiomasaEUPTContext();
                ordenesElaboracionesViewSource = (CollectionViewSource)(ucTablaElaboraciones.FindResource("ordenesElaboracionesViewSource"));
                productosTerminadosViewSource = (CollectionViewSource)(ucTablaProductosTerminados.FindResource("productosTerminadosViewSource"));

                context.OrdenesElaboraciones.Load();
                context.ProductosTerminados.Load();

                ordenesElaboracionesViewSource.Source = context.OrdenesElaboraciones.Local;
                productosTerminadosViewSource.Source = context.ProductosTerminados.Local;

            }
        }

        private void DgElaboraciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ordenElaboracion = (sender as DataGrid).SelectedItem as OrdenElaboracion;
            if (ordenElaboracion != null)
            {
                ucTablaProductosTerminados.IsEnabled = true;
                productosTerminadosViewSource.Source = context.ProductosTerminados.Where(pt => pt.OrdenId == ordenElaboracion.OrdenElaboracionId).ToList();

            }
            else
            {
                ucTablaProductosTerminados.IsEnabled = false;
            }
        }

        private async void BAnadirElaboracion_Click(object sender, RoutedEventArgs e)
        {
            var formElaboracion = new FormElaboracion(context);

            if ((bool)await DialogHost.Show(formElaboracion, "RootDialog"))
            {
                context.OrdenesElaboraciones.Add(new OrdenElaboracion()
                {
                    FechaElaboracion = new DateTime(formElaboracion.Fecha.Year, formElaboracion.Fecha.Month, formElaboracion.Fecha.Day, formElaboracion.Hora.Hour, formElaboracion.Hora.Minute, formElaboracion.Hora.Second),
                    EstadoElaboracionId = 1,
                    Descripcion = formElaboracion.Descripcion,
                    //EstadoId = (formRecepcion.cbEstadosRecepciones.SelectedItem as EstadoRecepcion).EstadoRecepcionId

                });
                context.SaveChanges();
            }
        }

        private async void BAnadirProductoTerminado_Click(object sender, RoutedEventArgs e)
        {
            var formProductoTerminado = new FormProductoTerminado(context);

            if ((bool)await DialogHost.Show(formProductoTerminado, "RootDialog"))
            {
                var productoTerminado = new ProductoTerminado()
                {
                    TipoId = (formProductoTerminado.cbTiposProductosTerminados.SelectedItem as TipoProductoTerminado).TipoProductoTerminadoId,
                    Volumen = formProductoTerminado.Volumen,
                    Unidades = formProductoTerminado.Unidades,
                    Observaciones = formProductoTerminado.Observaciones,

                };
                if (formProductoTerminado.FechaBaja != null)
                    productoTerminado.FechaBaja = new DateTime(formProductoTerminado.FechaBaja.Value.Year, formProductoTerminado.FechaBaja.Value.Month, formProductoTerminado.FechaBaja.Value.Day, formProductoTerminado.HoraBaja.Value.Hour, formProductoTerminado.HoraBaja.Value.Minute, formProductoTerminado.HoraBaja.Value.Second);

                context.ProductosTerminados.Add(productoTerminado);
                context.SaveChanges();
                //CollectionViewSource.GetDefaultView(ucTablaMateriasPrimas.dgMateriasPrimas.ItemsSource).Refresh();
                ucTablaProductosTerminados.dgProductosTerminados.Items.Refresh();
            }
        }

        public void FiltrarTablaElaboraciones()
        {
            ordenesElaboracionesViewSource.Filter += new FilterEventHandler(FiltroTablaElaboraciones);
        }

        private void FiltroTablaElaboraciones(object sender, FilterEventArgs e)
        {
            string textoBuscado = ucTablaElaboraciones.tbBuscar.Text.ToLower();
            var elaboracion = e.Item as OrdenElaboracion;

            string fechaElaboracion = elaboracion.FechaElaboracion.ToString();
            string estado = elaboracion.EstadoElaboracion.Nombre.ToLower();


            e.Accepted = (ucTablaElaboraciones.cbFechaElaboracion.IsChecked == true ? fechaElaboracion.Contains(textoBuscado) : false) ||
                         (ucTablaElaboraciones.cbEstado.IsChecked == true ? estado.Contains(textoBuscado) : false);

        }

        public void FiltrarTablaProductosTerminados()
        {
            productosTerminadosViewSource.Filter += new FilterEventHandler(FiltroTablaProductosTerminados);
        }

        private void FiltroTablaProductosTerminados(object sender, FilterEventArgs e)
        {
            string textoBuscado = ucTablaProductosTerminados.tbBuscar.Text.ToLower();
            var productoTerminado = e.Item as ProductoTerminado;
            string tipo = productoTerminado.TipoProductoTerminado.Nombre.ToLower();
            string grupo = productoTerminado.TipoProductoTerminado.GrupoProductoTerminado.Nombre.ToLower();
            string volumen = productoTerminado.Volumen.ToString();
            string unidades = productoTerminado.Unidades.ToString();

            e.Accepted = (ucTablaProductosTerminados.cbTipo.IsChecked == true ? tipo.Contains(textoBuscado) : false) ||
                         (ucTablaProductosTerminados.cbGrupo.IsChecked == true ? grupo.Contains(textoBuscado) : false) ||
                         (ucTablaProductosTerminados.cbVolUni.IsChecked == true ? (volumen.Contains(textoBuscado) || unidades.Contains(textoBuscado)) : false);
        }

        #region BorrarElaboracion
        private ICommand _borrarElaboracionComando;

        public ICommand BorrarElaboracionComando
        {
            get
            {
                if (_borrarElaboracionComando == null)
                {
                    _borrarElaboracionComando = new RelayComando(
                        param => BorrarElaboracion(),
                        param => CanBorrarElaboracion()
                    );
                }
                return _borrarElaboracionComando;
            }
        }

        private bool CanBorrarElaboracion()
        {
            if (ucTablaElaboraciones.dgElaboraciones.SelectedIndex != -1)
            {
                OrdenElaboracion ElaboracionSeleccionada = ucTablaElaboraciones.dgElaboraciones.SelectedItem as OrdenElaboracion;
                return ElaboracionSeleccionada != null;
            }
            return false;
        }

        private async void BorrarElaboracion()
        {
            string pregunta = ucTablaElaboraciones.dgElaboraciones.SelectedItems.Count == 1
                   ? "¿Está seguro de que desea borrar la elaboración " + (ucTablaElaboraciones.dgElaboraciones.SelectedItem as OrdenElaboracion).OrdenElaboracionId + "?"
                   : "¿Está seguro de que desea borrar las elaboraciones seleccionadas?";

            if ((bool)await DialogHost.Show(new MensajeConfirmacion(pregunta), "RootDialog"))
            {
                List<OrdenElaboracion> elaboracionesABorrar = new List<OrdenElaboracion>();
                var elaboracionesSeleccionadas = ucTablaElaboraciones.dgElaboraciones.SelectedItems.Cast<Recepcion>().ToList();
                foreach (var elaboracion in elaboracionesSeleccionadas)
                {
                    /*if (!context.OrdenesElaboraciones.Any(oe => oe.EstadoElaboracionId == elaboracion.EstadoId))
                    {
                        elaboracionesABorrar.Add(elaboracion);
                    }*/
                }
                context.OrdenesElaboraciones.RemoveRange(elaboracionesABorrar);
                context.SaveChanges();
                if (elaboracionesSeleccionadas.Count != elaboracionesABorrar.Count)
                {
                    string mensaje = ucTablaElaboraciones.dgElaboraciones.SelectedItems.Count == 1
                           ? "No se ha podido borrar la elaboración seleccionada."
                           : "No se han podido borrar todas las elaboración seleccionadas.";
                    mensaje += "\n\nAsegurese de no que no exista ningón producto terminado asociada a dicha elaboración.";
                    await DialogHost.Show(new MensajeInformacion(mensaje) { Width = 380 }, "RootDialog");
                }

            }
        }
        #endregion

        #region BorrarProductoTerminado
        private ICommand _borrarProductoTerminadoComando;

        public ICommand BorrarProductoTerminadoComando
        {
            get
            {
                if (_borrarProductoTerminadoComando == null)
                {
                    _borrarProductoTerminadoComando = new RelayComando(
                        param => BorrarProductoTerminado(),
                        param => CanBorrarProductoTerminado()
                    );
                }
                return _borrarProductoTerminadoComando;
            }
        }

        private bool CanBorrarProductoTerminado()
        {
            if (ucTablaProductosTerminados.dgProductosTerminados.SelectedIndex != -1)
            {
                ProductoTerminado productoTerminadoSeleccionado = ucTablaProductosTerminados.dgProductosTerminados.SelectedItem as ProductoTerminado;
                return productoTerminadoSeleccionado != null;
            }
            return false;
        }

        private async void BorrarProductoTerminado()
        {
            string pregunta = ucTablaProductosTerminados.dgProductosTerminados.SelectedItems.Count == 1
                ? "¿Está seguro de que desea borrar el producto terminado con código " + (ucTablaProductosTerminados.dgProductosTerminados.SelectedItem as ProductoTerminado).Codigo + "?"
                : "¿Está seguro de que desea borrar los productos terminados seleccionados?";

            if ((bool)await DialogHost.Show(new MensajeConfirmacion(pregunta), "RootDialog"))
            {

                List<ProductoTerminado> productosTerminadosABorrar = new List<ProductoTerminado>();
                var productosTerminadosSeleccionados = ucTablaProductosTerminados.dgProductosTerminados.SelectedItems.Cast<ProductoTerminado>().ToList();

                foreach (var productosTerminados in productosTerminadosSeleccionados)
                {
                    if (!context.OrdenesElaboraciones.Any(oe => oe.OrdenElaboracionId == productosTerminados.OrdenId))
                    {

                    }
                }
                context.ProductosTerminados.RemoveRange(ucTablaProductosTerminados.dgProductosTerminados.SelectedItems.Cast<ProductoTerminado>().ToList());
                context.SaveChanges();
                ucTablaProductosTerminados.dgProductosTerminados.Items.Refresh();
                // CollectionViewSource.GetDefaultView(ucTablaMateriasPrimas.dgMateriasPrimas.ItemsSource).Refresh();
            }
        }
        #endregion

        private async void RowElaboraciones_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var fila = sender as DataGridRow;
            var elaboracionSeleccionada = ucTablaElaboraciones.dgElaboraciones.SelectedItem as OrdenElaboracion;
            var formElaboracion = new FormElaboracion(context, "Editar Elaboración")
            {
                Fecha = elaboracionSeleccionada.FechaElaboracion.Value,
                Hora = elaboracionSeleccionada.FechaElaboracion.Value,
                Descripcion = elaboracionSeleccionada.Descripcion,
            };
            formElaboracion.cbEstadoElaboracion.Visibility = Visibility.Visible;
            formElaboracion.Fecha = elaboracionSeleccionada.FechaElaboracion.Value;
            formElaboracion.Hora = elaboracionSeleccionada.FechaElaboracion.Value;
            formElaboracion.cbEstadoElaboracion.SelectedValue = elaboracionSeleccionada.EstadoElaboracion.EstadoElaboracionId;
            if ((bool)await DialogHost.Show(formElaboracion, "RootDialog"))
            {
                elaboracionSeleccionada.FechaElaboracion = new DateTime(formElaboracion.Fecha.Year, formElaboracion.Fecha.Month, formElaboracion.Fecha.Day, formElaboracion.Hora.Hour, formElaboracion.Hora.Minute, formElaboracion.Hora.Second);
                elaboracionSeleccionada.EstadoElaboracionId = (formElaboracion.cbEstadoElaboracion.SelectedItem as EstadoElaboracion).EstadoElaboracionId;
                ordenesElaboracionesViewSource.View.Refresh();
                context.SaveChanges();
                /* using (var context = new BiomasaEUPTContext())
                 {
                     var recepcion = context.Recepciones.Single(tc => tc.NumeroAlbaran == albaranViejo);
                     recepcion.NumeroAlbaran = formTipo.Nombre;
                     tipoCliente.Descripcion = formTipo.Descripcion;
                     context.SaveChanges();
                 }*/
            }
        }




    }
}
