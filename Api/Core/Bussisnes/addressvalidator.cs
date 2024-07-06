using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
//PROYECTO


 namespace storeapi.Bussisnes


{
    


public class AddressValidator
{
    public Dictionary<string, string> Validate(string address)
    {
        var errors = new Dictionary<string, string>();

        var addressParts = address.Split(", ");
        if (addressParts.Length != 5)
        {
            errors["Formato"] = "La dirección debe tener cinco partes: nombre del destinatario, calle, número, ciudad y país.";
            return errors;
        }

        var nombreDestinatario = addressParts[0];
        var direccionCalle = addressParts[1];
        var direccionNumero = addressParts[2];
        var direccionCiudad = addressParts[3];
        var direccionPais = addressParts[4];

        // Validación de Nombre del Destinatario
        if (nombreDestinatario.Length < 5)
        {
            errors["NombreDestinatario"] = "El nombre del destinatario debe tener al menos 5 caracteres.";
        }
        else if (!Regex.IsMatch(nombreDestinatario, @"^[a-zA-Z\sñÑáéíóúÁÉÍÓÚ]+$"))
        {
            errors["NombreDestinatario"] = "El nombre solo debe contener letras y espacios.";
        }

        // Validación de Dirección Calle
        if (direccionCalle.Length < 5)
        {
            errors["DireccionCalle"] = "La calle debe tener al menos 5 caracteres.";
        }
        else if (!Regex.IsMatch(direccionCalle, @"^[a-zA-Z0-9\sñÑáéíóúÁÉÍÓÚ.,'-]+$"))
        {
            errors["DireccionCalle"] = "La dirección contiene caracteres inválidos.";
        }

        // Validación de Dirección Número
        if (direccionNumero.Length < 1)
        {
            errors["DireccionNumero"] = "El número debe tener al menos 1 carácter.";
        }
        else if (!Regex.IsMatch(direccionNumero, @"^\d+[a-zA-Z]?$"))
        {
            errors["DireccionNumero"] = "El número de la casa es inválido.";
        }

        // Validación de Dirección Ciudad
        if (direccionCiudad.Length < 3)
        {
            errors["DireccionCiudad"] = "La ciudad debe tener al menos 3 caracteres.";
        }
        else if (!Regex.IsMatch(direccionCiudad, @"^[a-zA-Z\sñÑáéíóúÁÉÍÓÚ]+$"))
        {
            errors["DireccionCiudad"] = "El nombre de la ciudad solo debe contener letras y espacios.";
        }

        // Validación de Dirección País
        if (direccionPais.Length < 3)
        {
            errors["DireccionPais"] = "El país debe tener al menos 3 caracteres.";
        }
        else if (!Regex.IsMatch(direccionPais, @"^[a-zA-Z\sñÑáéíóúÁÉÍÓÚ]+$"))
        {
            errors["DireccionPais"] = "El nombre del país solo debe contener letras y espacios.";
        }

        return errors;
    }
}
}