﻿using BiomasaEUPT.Modelos;
using BiomasaEUPT.Vistas;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BiomasaEUPT
{
    /// <summary>
    /// Lógica de interacción para Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        private SplashViewModel viewModel;
        public Splash()
        {
            InitializeComponent();
            viewModel = new SplashViewModel();
            DataContext = viewModel;
            //IniciarConfig();
            //IniciarCarpetas();
        }

        private void IniciarConfig()
        {
            if (!File.Exists("BiomasaEUPT.exe.config"))
            {
                File.WriteAllText(@"BiomasaEUPT.exe.config", Properties.Resources.App);
            }
        }

        private void IniciarCarpetas()
        {
            if (!Directory.Exists("carpeta"))
            {
                Directory.CreateDirectory("carpeta");
            }
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            await Task.Run(() => IniciarPrograma());

            Login login = new Login();
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.usuario) && !string.IsNullOrWhiteSpace(Properties.Settings.Default.contrasena))
            {
                var usuario = login.ViewModel.IniciarSesion(Properties.Settings.Default.usuario, Properties.Settings.Default.contrasena);
                if (usuario != null)
                {
                    login.Close();
                    MainWindow main = new MainWindow();
                    (main.DataContext as MainWindowViewModel).Usuario = usuario;
                    await Task.Run(() => InicioFinalizado());
                    Close();
                    main.Show();

                }
                else
                {
                    login.Show();
                    login.tbUsuario.Text = Properties.Settings.Default.usuario;
                    Close();
                    login.ViewModel.MensajeLoginIncorrecto();
                }
            }
            else
            {
                login.Show();
                login.tbUsuario.Text = Properties.Settings.Default.usuario;
                Close();
            }


        }

        private void IniciarPrograma()
        {
            // Estado 1 - Actualización
            //Properties.Settings.Default.ActualizarPrograma = true;
            if (Properties.Settings.Default.ActualizarPrograma)
            {
                var actualizador = new Actualizador()
                {
                    SplashViewModel = viewModel
                };


                if (actualizador.ComprobarActualizacionPrograma())
                {
                    actualizador.ActualizarPrograma();
                }
            }

            // Estado 2 - Conexión BD
            Dispatcher.Invoke(() =>
            {
                viewModel.MensajeInformacion = "Conectándose a la BD...";
                viewModel.Progreso = 50;

            });

            using (var context = new BiomasaEUPTContext())
            {
                try { context.Database.Connection.Open(); }
                catch
                {                    
                    Dispatcher.Invoke(() =>
                    {
                        viewModel.MensajeInformacion = "No se ha podido conectar con la Base de Datos. Saliendo...";
                        viewModel.Progreso = 100;
                    });
                    Thread.Sleep(2000);

                    /// Cierra la aplicación
                    Process.GetCurrentProcess().Kill();
                }
            }

            // Thread.Sleep(500);
        }

        private void InicioFinalizado()
        {
            Dispatcher.Invoke(() =>
            {
                viewModel.MensajeInformacion = "Iniciando...";
                viewModel.Progreso = 100;

            });

            // Thread.Sleep(1000);
        }
    }
}
