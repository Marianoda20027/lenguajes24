using System;
using System.Collections.Generic;

namespace storeapi.Models
{
    public class Campanna
    {
        public int Id { get; set; }

        private string _contenidoHtml = string.Empty;
        private bool _estado;

        public string ContenidoHtml
        {
            get => _contenidoHtml;
            set
            {
                if (!ValidateContenidoHtml(value))
                {
                    throw new ArgumentException("ContenidoHtml must not be null, empty, or exceed 5000 characters.");
                }
                _contenidoHtml = value;
            }
        }

        public bool Estado
        {
            get => _estado;
            set
            {
                if (!ValidateEstado(value))
                {
                    throw new ArgumentException("Estado must be a valid boolean value.");
                }
                _estado = value;
            }
        }

        public DateTime CreatedAt { get; set; }

        public Campanna(string contenidoHtml)
        {
            ContenidoHtml = contenidoHtml;
            Estado = true; // Default value
            CreatedAt = DateTime.UtcNow; // Default value
        }

        public Campanna() { }

        public static bool ValidateContenidoHtml(string contenidoHtml)
        {
            return !string.IsNullOrWhiteSpace(contenidoHtml) && contenidoHtml.Length <= 5000;
        }

        public static bool ValidateEstado(bool estado)
        {
            return estado == true || estado == false;
        }

        public static bool ValidateCampannaRow(string[] row)
        {
            if (row.Length != 4)
            {
                return false;
            }

            if (!int.TryParse(row[0], out _))
            {
                return false;
            }

            if (!ValidateContenidoHtml(row[1]))
            {
                return false;
            }

            if (!DateTime.TryParse(row[2], out _))
            {
                return false;
            }

            if (!bool.TryParse(row[3], out _))
            {
                return false;
            }

            return true;
        }
    }
}

