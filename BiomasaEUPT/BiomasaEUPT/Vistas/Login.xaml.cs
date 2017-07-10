﻿using BiomasaEUPT.Clases;
using BiomasaEUPT.Domain;
using BiomasaEUPT.Modelos;
using BiomasaEUPT.Modelos.Tablas;
using BiomasaEUPT.Vistas;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
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
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private string _usuario;
        public string Usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }

        private SecureString _contrasena;
        public SecureString Contrasena
        {
            get { return _contrasena; }
            set
            {
                _contrasena = value;
            }
        }

        public Login()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void pbContrasena_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pBox = sender as PasswordBox;
            PasswordBoxAttachedProperties.SetEncryptedPassword(pBox, pBox.SecurePassword);
        }

        private void CargarVistaMain()
        {
            MainWindow mainWindows = new MainWindow();
            Close();
            mainWindows.Show();
        }

        public Usuario IniciarSesion(String usuario, String hashContrasena)
        {
            using (var context = new BiomasaEUPTContext())
            {
                return context.Usuarios.FirstOrDefault(u => u.Nombre == usuario && u.Contrasena == hashContrasena
                                                            && u.Baneado == false);
            }
        }

        private void bIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            String hashContrasena = "";
            if (Contrasena != null)
            {
                hashContrasena = ContrasenaHashing.obtenerHashSHA256(ContrasenaHashing.SecureStringToString(Contrasena));
            }

            if (IniciarSesion(Usuario, hashContrasena) != null)
            {
                Properties.Settings.Default.contrasena = cbRecordarme.IsChecked == true ? hashContrasena : "";
                Properties.Settings.Default.usuario = Usuario;
                Properties.Settings.Default.Save();
                CargarVistaMain();
            }
            else
            {
                MensajeLoginIncorrecto();
            }

        }

        public async void MensajeLoginIncorrecto()
        {
            //MessageBox.Show("El usuario y/o la contraseña son incorrectos.", "Login incorrecto", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            var mensaje = new MensajeInformacion()
            {
                Titulo = "Login incorrecto",
                Mensaje = "El usuario y/o la contraseña son incorrectos."
            };
            var resultado = await DialogHost.Show(mensaje, "RootDialog");
            // Console.WriteLine("*******" + mensaje.DataContext.GetType().GetProperty("Nombre").GetValue(mensaje.DataContext));
        }
    }
}
