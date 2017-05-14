﻿using BiomasaEUPT.Clases;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace BiomasaEUPT.Modelos.Validadores
{
    public class ClienteValidationRule : ValidationRule
    {
        private int minRazonSocial = 5;
        private int maxRazonSocial = 100;
        private string regexRazonSocial = "^(?!\\s)(?!.*\\s$)[\\p{L}0-9\\s'~?!\\.,@]+$";
        private int minNif = 9;
        private int maxNif = 9;
        private string regexNif = "^([A-Z]-\\d{7})|(\\d{7}-[A-Z])$";
        private int minEmail = 10;
        private int maxEmail = 254;
        private string regexEmail = "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
        private int minCalle = 20;
        private int maxCalle = 100;
        private string regexCalle = "^(?!\\s)(?!.*\\s$)[\\p{L}0-9\\s'~?!\\.,\\/]+$";
        //private int minObservaciones;
        //private int maxObservaciones;

        private string errorObligatorio = "El campo {0} es obligatorio.";
        private string errorMin = "La longitud del campo {0} es menor de {1} carácteres.";
        private string errorMax = "La longitud del campo {0} es mayor de {1} carácteres.";
        private string errorUnico = "El campo {0} debe ser único.";
        private string errorRegex = "El campo {0} no tiene formato válido{1}.";


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            clientes cliente = (value as BindingGroup).Items[0] as clientes;

            // Razón Social
            if (string.IsNullOrWhiteSpace(cliente.razon_social))
                return new ValidationResult(false, String.Format(errorObligatorio, "razón social"));

            if (cliente.razon_social.Length < minRazonSocial)
                return new ValidationResult(false, String.Format(errorMin, "razón social", minRazonSocial));

            if (cliente.razon_social.Length > maxRazonSocial)
                return new ValidationResult(false, String.Format(errorMax, "razón social", maxRazonSocial));

            if (!Regex.IsMatch(cliente.razon_social, regexRazonSocial))
                return new ValidationResult(false, String.Format(errorRegex, "razón social"));


            // NIF
            if (string.IsNullOrWhiteSpace(cliente.nif))
                return new ValidationResult(false, String.Format(errorObligatorio, "NIF"));

            if (cliente.nif.Length < minNif)
                return new ValidationResult(false, String.Format(errorMin, "NIF", minRazonSocial));

            if (cliente.nif.Length > maxNif)
                return new ValidationResult(false, String.Format(errorMax, "NIF", maxRazonSocial));

            if (!Regex.IsMatch(cliente.nif, regexNif))
                return new ValidationResult(false, String.Format(errorRegex, "NIF", " (L-NNNNNNN o NNNNNNN-L)"));


            // Email
            if (string.IsNullOrWhiteSpace(cliente.email))
                return new ValidationResult(false, String.Format(errorObligatorio, "email"));

            if (cliente.email.Length < minEmail)
                return new ValidationResult(false, String.Format(errorMin, "email", minEmail));

            if (cliente.email.Length > maxEmail)
                return new ValidationResult(false, String.Format(errorMax, "email", maxEmail));

            if (!Regex.IsMatch(cliente.email, regexEmail))
                return new ValidationResult(false, String.Format(errorRegex, "email"));


            // Calle
            if (string.IsNullOrWhiteSpace(cliente.calle))
                return new ValidationResult(false, String.Format(errorObligatorio, "calle"));

            if (cliente.calle.Length < minCalle)
                return new ValidationResult(false, String.Format(errorMin, "calle", minCalle));

            if (cliente.calle.Length > maxCalle)
                return new ValidationResult(false, String.Format(errorMax, "calle", maxCalle));

            if (!Regex.IsMatch(cliente.calle, regexCalle))
                return new ValidationResult(false, String.Format(errorRegex, "calle"));


            // Valores únicos
            foreach (var c in BaseDeDatos.Instancia.biomasaEUPTEntidades.clientes.Local)
            {
                if (c.GetHashCode() != cliente.GetHashCode())
                {
                    if (c.razon_social == cliente.razon_social)
                        return new ValidationResult(false, String.Format(errorUnico, "razón social"));

                    if (c.nif == cliente.nif)
                        return new ValidationResult(false, String.Format(errorUnico, "NIF"));

                    if (c.email == cliente.email)
                        return new ValidationResult(false, String.Format(errorUnico, "email"));
                }
            }

            return ValidationResult.ValidResult;
        }
    }
}
