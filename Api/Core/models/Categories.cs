using System;
using System.Collections.Generic;
using System.Linq;

namespace storeapi.Models
{
    public struct Category
    {

        public  int _id;
        public  string _name;

        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id
        {
            get => _id;
            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Category Id must be greater than zero.", nameof(Id));
                }
                _id = value;
            }
        }

        public string Name
        {
            get => _name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Category Name must not be null or empty.", nameof(Name));
                }
                _name = value;
            }
        }

    }

    public class Categories
    {
        public List<Category> ListCategories { get; set; } = new List<Category>
        {
            new Category(1, "Electrónica"),
            new Category(2, "Moda"),
            new Category(3, "Hogar y jardín"),
            new Category(4, "Deportes y actividades al aire libre"),
            new Category(5, "Belleza y cuidado personal"),
            new Category(6, "Alimentación y bebidas"),
            new Category(7, "Libros y entretenimiento"),
            new Category(8, "Tecnología"),
            new Category(9, "Deportes")
        };

        public Categories()
        {
            ListCategories.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
        }

        public Category GetCategoryById(int categoryId)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentException("Invalid categoryId. It must be greater than zero.", nameof(categoryId));
            }

            var category = ListCategories.FirstOrDefault(c => c.Id == categoryId);


            return category;
        }

    }
}

