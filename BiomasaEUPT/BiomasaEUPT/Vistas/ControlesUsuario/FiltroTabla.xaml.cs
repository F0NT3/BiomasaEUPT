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
using System.Data.Entity;

namespace BiomasaEUPT.Vistas.ControlesUsuario
{
    /// <summary>
    /// Lógica de interacción para FiltroTabla.xaml
    /// </summary>
    public partial class FiltroTabla : UserControl
    {
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
            //DataContext = this;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!MostrarGrupo)
            {
                czGrupos.Visibility = Visibility.Collapsed;
            }

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
                // tabClientes.FiltrarTabla();
            }
            // Pestaña Proveedores
            if (ucParent.GetType().Equals(typeof(TabProveedores)))
            {
                TabProveedores tabProveedores = (TabProveedores)ucParent;
                tiposProveedoresViewSource = ((CollectionViewSource)(tabProveedores.ucTablaProveedores.FindResource("tiposProveedoresViewSource")));
                ccFiltroTipo.Collection = tiposProveedoresViewSource.View;
                // tabProveedores.FiltrarTabla();
            }
        }


        private async void bAnadirTipo_Click(object sender, RoutedEventArgs e)
        {
            var formTipo = new FormTipo();
            formTipo.vNombreUnico.Atributo = "Nombre";

            // Pestaña Clientes
            if (ucParent.GetType().Equals(typeof(TabClientes)))
            {
                formTipo.vNombreUnico.Coleccion = tiposClientesViewSource;
                formTipo.vNombreUnico.Tipo = "TipoCliente";
            }

            // Pestaña Proveedores
            if (ucParent.GetType().Equals(typeof(TabProveedores)))
            {
                formTipo.vNombreUnico.Coleccion = tiposProveedoresViewSource;
                formTipo.vNombreUnico.Tipo = "TiposProveedor";
            }

            if ((bool)await DialogHost.Show(formTipo, "RootDialog"))
            {
                using (var context = new BiomasaEUPTContext())
                {
                    if (ucParent.GetType().Equals(typeof(TabClientes)))
                    {
                        context.TiposClientes.Add(new TipoCliente() { Nombre = formTipo.Nombre, Descripcion = formTipo.Descripcion });
                        TabClientes tabClientes = (TabClientes)ucParent;
                        tabClientes.CargarClientes();
                    }
                    if (ucParent.GetType().Equals(typeof(TabProveedores)))
                    {
                        context.TiposProveedores.Add(new TipoProveedor() { Nombre = formTipo.Nombre, Descripcion = formTipo.Descripcion });
                        TabProveedores tabProveedores = (TabProveedores)ucParent;
                    }
                    context.SaveChanges();
                }
                RefrescarListaTipos();
            }
        }

        private async void bEditarTipo_Click(object sender, RoutedEventArgs e)
        {
            var formTipo = new FormTipo("Editar Tipo");
            formTipo.vNombreUnico.Atributo = "Nombre";

            // Pestaña Clientes
            if (ucParent.GetType().Equals(typeof(TabClientes)))
            {
                var tipoSeleccionado = lbFiltroTipo.SelectedItem as TipoCliente;
                formTipo.Nombre = tipoSeleccionado.Nombre;
                formTipo.Descripcion = tipoSeleccionado.Descripcion;
                var nombreViejo = formTipo.Nombre;
                formTipo.vNombreUnico.Coleccion = tiposClientesViewSource;
                formTipo.vNombreUnico.Tipo = "TipoCliente";
                formTipo.vNombreUnico.NombreActual = tipoSeleccionado.Nombre;
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
                formTipo.vNombreUnico.Tipo = "TipoProveedor";
                formTipo.vNombreUnico.NombreActual = tipoSeleccionado.Nombre;
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
                    RefrescarListaTipos();
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
                        if (!context.Clientes.Any(t => t.TipoId == tipoSeleccionado.TipoClienteId))
                        {
                            var tipo = context.TiposClientes.Where(tc => tc.TipoClienteId == tipoSeleccionado.TipoClienteId).First();
                            context.TiposClientes.Remove(tipo);
                            context.SaveChanges();
                        }
                        else
                        {
                            await DialogHost.Show(new MensajeInformacion("No puede borrar el tipo debido a que está en uso"), "RootDialog");
                        }
                    }
                    RefrescarListaTipos();
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
                        if (!context.Proveedores.Any(t => t.TipoId == tipoSeleccionado.TipoProveedorId))
                        {
                            var tipo = context.TiposProveedores.Where(tc => tc.TipoProveedorId == tipoSeleccionado.TipoProveedorId).First();
                            context.TiposProveedores.Remove(tipo);
                            context.SaveChanges();
                        }
                        else
                        {
                            await DialogHost.Show(new MensajeInformacion("No puede borrar el tipo debido a que está en uso"), "RootDialog");
                        }
                    }
                    RefrescarListaTipos();
                }
            }
        }

        private async void bAnadirGrupo_Click(object sender, RoutedEventArgs e)
        {
            var formGrupo = new FormTipo("Nuevo Grupo");
            formGrupo.vNombreUnico.Atributo = "Nombre";

            // Pestaña Clientes
            if (ucParent.GetType().Equals(typeof(TabClientes)))
            {
                formGrupo.vNombreUnico.Coleccion = gruposClientesViewSource;
                formGrupo.vNombreUnico.Tipo = "GrupoCliente";
            }

            if ((bool)await DialogHost.Show(formGrupo, "RootDialog"))
            {
                using (var context = new BiomasaEUPTContext())
                {
                    if (ucParent.GetType().Equals(typeof(TabClientes)))
                        context.GruposClientes.Add(new GrupoCliente() { Nombre = formGrupo.Nombre, Descripcion = formGrupo.Descripcion });

                    context.SaveChanges();
                }
                RefrescarListaGrupos();
            }
        }

        private async void bEditarGrupo_Click(object sender, RoutedEventArgs e)
        {
            var formGrupo = new FormTipo("Editar Grupo");
            formGrupo.vNombreUnico.Atributo = "Nombre";

            // Pestaña Clientes
            if (ucParent.GetType().Equals(typeof(TabClientes)))
            {
                var grupoSeleccionado = lbFiltroGrupo.SelectedItem as GrupoCliente;
                formGrupo.Nombre = grupoSeleccionado.Nombre;
                formGrupo.Descripcion = grupoSeleccionado.Descripcion;
                var nombreViejo = formGrupo.Nombre;
                formGrupo.vNombreUnico.Coleccion = gruposClientesViewSource;
                formGrupo.vNombreUnico.Tipo = "GrupoCliente";
                formGrupo.vNombreUnico.NombreActual = grupoSeleccionado.Nombre;
                if ((bool)await DialogHost.Show(formGrupo, "RootDialog"))
                {
                    grupoSeleccionado.Nombre = formGrupo.Nombre;
                    grupoSeleccionado.Descripcion = formGrupo.Descripcion;
                    gruposClientesViewSource.View.Refresh();
                    using (var context = new BiomasaEUPTContext())
                    {
                        var grupoCliente = context.GruposClientes.Single(gc => gc.Nombre == nombreViejo);
                        grupoCliente.Nombre = formGrupo.Nombre;
                        grupoCliente.Descripcion = formGrupo.Descripcion;
                        context.SaveChanges();
                    }
                    RefrescarListaGrupos();
                }
            }
        }

        private async void bBorrarGrupo_Click(object sender, RoutedEventArgs e)
        {
            var mensajeConf = new MensajeConfirmacion();

            // Pestaña Clientes
            if (ucParent.GetType().Equals(typeof(TabClientes)))
            {
                var grupoSeleccionado = lbFiltroGrupo.SelectedItem as GrupoCliente;
                mensajeConf.Mensaje = "¿Está seguro de que desea borrar el grupo " + grupoSeleccionado.Nombre + "?";
                if ((bool)await DialogHost.Show(mensajeConf, "RootDialog"))
                {
                    using (var context = new BiomasaEUPTContext())
                    {
                        if (!context.GruposClientes.Any(gc => gc.GrupoClienteId == grupoSeleccionado.GrupoClienteId))
                        {
                            var grupo = context.GruposClientes.Where(gc => gc.GrupoClienteId == grupoSeleccionado.GrupoClienteId).First();
                            context.GruposClientes.Remove(grupo);
                            context.SaveChanges();
                        }
                        else
                        {
                            await DialogHost.Show(new MensajeInformacion("No puede borrar el grupo debido a que está en uso"), "RootDialog");
                        }
                    }
                    RefrescarListaGrupos();
                }
            }
        }

        private void RefrescarListaTipos()
        {
            using (var context = new BiomasaEUPTContext())
            {
                if (ucParent.GetType().Equals(typeof(TabClientes)))
                {
                    context.TiposClientes.Load();
                    tiposClientesViewSource.Source = context.TiposClientes.Local;
                    tiposClientesViewSource.View.Refresh();
                    ccFiltroTipo.Collection = tiposClientesViewSource.View;
                }
                if (ucParent.GetType().Equals(typeof(TabProveedores)))
                {
                    context.TiposProveedores.Load();
                    tiposProveedoresViewSource.Source = context.TiposProveedores.Local;
                    tiposProveedoresViewSource.View.Refresh();
                    ccFiltroTipo.Collection = tiposProveedoresViewSource.View;
                }
            }
        }

        private void RefrescarListaGrupos()
        {
            using (var context = new BiomasaEUPTContext())
            {
                if (ucParent.GetType().Equals(typeof(TabClientes)))
                {
                    context.GruposClientes.Load();
                    gruposClientesViewSource.Source = context.GruposClientes.Local;
                    gruposClientesViewSource.View.Refresh();
                    ccFiltroGrupo.Collection = gruposClientesViewSource.View;
                }
            }
        }
    }
}
