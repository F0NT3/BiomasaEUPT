﻿using BiomasaEUPT.Clases;
using BiomasaEUPT.Modelos.Validadores;
using BiomasaEUPT.Vistas.GestionClientes;
using BiomasaEUPT.Vistas.GestionProveedores;
using BiomasaEUPT.Vistas.GestionRecepciones;
using BiomasaEUPT.Vistas.GestionUsuarios;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BiomasaEUPT.Modelos;
using BiomasaEUPT.Modelos.Tablas;

namespace BiomasaEUPT.Vistas.ControlesUsuario
{
    /// <summary>
    /// Lógica de interacción para FiltroTabla.xaml
    /// </summary>
    public partial class FiltroTabla : UserControl
    {
        //private BiomasaEUPTContext context;
        private DependencyObject ucParent;
        private CollectionViewSource tiposClientesViewSource;
        private CollectionViewSource tiposProveedoresViewSource;

        private CollectionViewSource gruposClientesViewSource;

        public bool MostrarGrupo { get; set; } = true;
        /* public static readonly DependencyProperty MostrarGrupoProperty = DependencyProperty.Register
            (
                 "MostrarGrupo",
                 typeof(bool),
                 typeof(FiltroTabla),
                 new PropertyMetadata(true)
            );

         public bool MostrarGrupo
         {
             get { return (bool)GetValue(MostrarGrupoProperty); }
             set { SetValue(MostrarGrupoProperty, value); }
         }*/


        public FiltroTabla()
        {
            InitializeComponent();
            DataContext = this;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!MostrarGrupo)
            {
                czGrupos.Visibility = Visibility.Collapsed;
            }

            //context = BaseDeDatos.Instancia.biomasaEUPTContext;
            ucParent = Parent;
            while (!(ucParent is UserControl))
            {
                ucParent = LogicalTreeHelper.GetParent(ucParent);
            }

            // Pestaña Clientes
            if (ucParent.GetType().Equals(typeof(TabClientes)))
            {
                TabClientes tabClientes = (TabClientes)ucParent;
                tiposClientesViewSource = ((CollectionViewSource)(tabClientes.ucTablaClientes.FindResource("tiposClientesViewSource")));
                ccFiltroTipo.Collection = tiposClientesViewSource.View;
                gruposClientesViewSource = ((CollectionViewSource)(tabClientes.ucTablaClientes.FindResource("gruposClientesViewSource")));
                ccFiltroGrupo.Collection = gruposClientesViewSource.View;
                //   tabClientes.FiltrarTabla();
            }
            // Pestaña Proveedores
            if (ucParent.GetType().Equals(typeof(TabProveedores)))
            {
                TabProveedores tabProveedores = (TabProveedores)ucParent;
                tiposProveedoresViewSource = ((CollectionViewSource)(tabProveedores.ucTablaProveedores.FindResource("tiposProveedoresViewSource")));
                ccFiltroTipo.Collection = tiposProveedoresViewSource.View;
                //   tabProveedores.FiltrarTabla();
            }
        }


        private async void bAnadirTipo_Click(object sender, RoutedEventArgs e)
        {
            var formTipo = new FormTipo();
            formTipo.vNombreUnico.Atributo = "nombre";

            // Pestaña Clientes
            if (ucParent.GetType().Equals(typeof(TabClientes)))
            {
                //TabClientes tabClientes = (TabClientes)ucParent;
                formTipo.vNombreUnico.Coleccion = tiposClientesViewSource;
                formTipo.vNombreUnico.Tipo = "tiposClientes";
            }

            // Pestaña Proveedores
            if (ucParent.GetType().Equals(typeof(TabProveedores)))
            {
                //TabProveedores tabProveedores = (TabProveedores)ucParent;
                formTipo.vNombreUnico.Coleccion = tiposProveedoresViewSource;
                formTipo.vNombreUnico.Tipo = "tiposProveedores";
            }

            if ((bool)await DialogHost.Show(formTipo, "RootDialog"))
            {
                using (var context = new BiomasaEUPTContext())
                {
                    if (ucParent.GetType().Equals(typeof(TabClientes)))
                        context.TiposClientes.Add(new TipoCliente() { Nombre = formTipo.Nombre, Descripcion = formTipo.Descripcion });

                    if (ucParent.GetType().Equals(typeof(TabProveedores)))
                        context.TiposProveedores.Add(new TipoProveedor() { Nombre = formTipo.Nombre, Descripcion = formTipo.Descripcion });

                    context.SaveChanges();
                }
            }


        }

        private async void bEditarTipo_Click(object sender, RoutedEventArgs e)
        {
            var formTipo = new FormTipo();
            formTipo.vNombreUnico.Atributo = "nombre";

            // Pestaña Clientes
            if (ucParent.GetType().Equals(typeof(TabClientes)))
            {
                var tipoSeleccionado = lbFiltroTipo.SelectedItem as TipoCliente;
                formTipo.Nombre = tipoSeleccionado.Nombre;
                formTipo.Descripcion = tipoSeleccionado.Descripcion;
                var nombreViejo = formTipo.Nombre;
                formTipo.vNombreUnico.Coleccion = tiposClientesViewSource;
                formTipo.vNombreUnico.Tipo = "tiposClientes";
                formTipo.vNombreUnico.NombreActual = (lbFiltroTipo.SelectedItem as TipoCliente).Nombre;
                if ((bool)await DialogHost.Show(formTipo, "RootDialog"))
                {
                    tipoSeleccionado.Nombre = formTipo.Nombre;
                    tipoSeleccionado.Descripcion = formTipo.Descripcion;
                    tiposClientesViewSource.View.Refresh();
                    using (var context = new BiomasaEUPTContext())
                    {
                        var tipoCliente = context.TiposClientes.Single(tc => tc.Nombre == nombreViejo);
                        tipoCliente.Nombre = formTipo.Nombre;
                        tipoCliente.Descripcion = formTipo.Descripcion;
                        context.SaveChanges();
                    }
                }
            }

            // Pestaña Proveedores
            if (ucParent.GetType().Equals(typeof(TabProveedores)))
            {
                var tipoSeleccionado = lbFiltroTipo.SelectedItem as TipoProveedor;
                formTipo.Nombre = tipoSeleccionado.Nombre;
                formTipo.Descripcion = tipoSeleccionado.Descripcion;
                var nombreViejo = formTipo.Nombre;
                formTipo.vNombreUnico.Coleccion = tiposClientesViewSource;
                formTipo.vNombreUnico.Tipo = "tiposProveedores";
                formTipo.vNombreUnico.NombreActual = (lbFiltroTipo.SelectedItem as TipoProveedor).Nombre;
                if ((bool)await DialogHost.Show(formTipo, "RootDialog"))
                {
                    tipoSeleccionado.Nombre = formTipo.Nombre;
                    tipoSeleccionado.Descripcion = formTipo.Descripcion;
                    tiposClientesViewSource.View.Refresh();
                    using (var context = new BiomasaEUPTContext())
                    {
                        var tipoProveedor = context.TiposProveedores.Single(tc => tc.Nombre == nombreViejo);
                        tipoProveedor.Nombre = formTipo.Nombre;
                        tipoProveedor.Descripcion = formTipo.Descripcion;
                        context.SaveChanges();
                    }
                }
            }

        }

        private async void bBorrarTipo_Click(object sender, RoutedEventArgs e)
        {
            var mensajeConf = new MensajeConfirmacion();

            // Pestaña Clientes
            if (ucParent.GetType().Equals(typeof(TabClientes)))
            {
                var tipoSeleccionado = lbFiltroTipo.SelectedItem as TipoCliente;
                mensajeConf.Mensaje = "¿Está seguro de que desea borrar el tipo " + tipoSeleccionado.Nombre + "?";
                if ((bool)await DialogHost.Show(mensajeConf, "RootDialog"))
                {
                    using (var context = new BiomasaEUPTContext())
                    {
                        if (context.Clientes.Where(t => t.TipoId == tipoSeleccionado.TipoClienteId).Count() == 0)
                        {
                            context.TiposClientes.Remove(tipoSeleccionado);
                            //context.GuardarCambios<TipoCliente>();
                            context.SaveChanges();
                        }
                        else
                        {
                            await DialogHost.Show(new MensajeInformacion("No puede borrar el tipo debido a que está en uso"), "RootDialog");
                        }
                    }
                }
            }

            // Pestaña Proveedores
            if (ucParent.GetType().Equals(typeof(TabProveedores)))
            {
                var tipoSeleccionado = lbFiltroTipo.SelectedItem as TipoProveedor;
                mensajeConf.Mensaje = "¿Está seguro de que desea borrar el tipo " + tipoSeleccionado.Nombre + "?";
                if ((bool)await DialogHost.Show(mensajeConf, "RootDialog"))
                {
                    using (var context = new BiomasaEUPTContext())
                    {
                        if (context.Clientes.Where(t => t.TipoId == tipoSeleccionado.TipoProveedorId).Count() == 0)
                        {
                            context.TiposProveedores.Remove(tipoSeleccionado);
                            //context.GuardarCambios<TipoProveedor>();
                            context.SaveChanges();
                        }
                        else
                        {
                            await DialogHost.Show(new MensajeInformacion("No puede borrar el tipo debido a que está en uso"), "RootDialog");
                        }
                    }
                }
            }

            // Pestaña Materias primas y Recepciones
            if (ucParent.GetType().Equals(typeof(TabRecepciones)))
            {
                var tipoSeleccionado = lbFiltroTipo.SelectedItem as TipoMateriaPrima;
                mensajeConf.Mensaje = "¿Está seguro de que desea borrar el tipo " + tipoSeleccionado.Nombre + "?";
                if ((bool)await DialogHost.Show(mensajeConf, "RootDialog"))
                {
                    using (var context = new BiomasaEUPTContext())
                    {
                        if (context.Clientes.Where(t => t.TipoId == tipoSeleccionado.TipoMateriaPrimaId).Count() == 0)
                        {
                            context.TiposMateriasPrimas.Remove(tipoSeleccionado);
                            //context.GuardarCambios<TipoMateriaPrima>();
                            context.SaveChanges();
                        }
                        else
                        {
                            await DialogHost.Show(new MensajeInformacion("No puede borrar el tipo debido a que está en uso"), "RootDialog");
                        }
                    }
                }
            }

        }

        private void bAnadirGrupo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bEditarGrupo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bBorrarGrupo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
