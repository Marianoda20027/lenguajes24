using System;

namespace storeapi.Models
{
    public class Product : ICloneable
    {
        public int id { get; set; }
        private string _name = string.Empty; // Initialize to empty string
        private string _imageUrl = string.Empty; // Initialize to empty string
        private decimal _price;
        private string _description = string.Empty; // Initialize to empty string
        private Category _category ;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name must not be null or empty.");
                }
                _name = value;
            }
        }

        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("ImageUrl must not be null or empty.");
                }
                _imageUrl = value;
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Price must be a positive decimal value.");
                }
                _price = value;
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Description must not be null or empty.");
                }
                _description = value;
            }
        }

         public Category Category
    {
        get => _category;
        set
        {
            // Validar que el Id sea mayor que cero
            if (value.Id <= 0)
            {
                throw new ArgumentException("El Id de Category debe ser mayor que cero.");
            }

            // Validar que el nombre no esté vacío
            if (string.IsNullOrWhiteSpace(value.Name))
            {
                throw new ArgumentException("El nombre de Category no puede estar vacío o ser nulo.");
            }

            // Asignar el valor solo si pasa las validaciones
            _category = value;
        }
    }

        public object Clone()
        {
            // Create a new instance of Product and copy all properties
            return new Product
            {
              
                Name = this.Name,
                ImageUrl = this.ImageUrl,
                Price = this.Price,
                Description = this.Description,
                Category = this.Category // Assign the category directly
            };
        }
    }
}
