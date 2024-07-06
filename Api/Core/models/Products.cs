using System;
using System.Collections.Generic;
using System.Linq;
using storeapi.Database;
using System.Globalization;

namespace storeapi.Models
{
    public sealed class Products
    {
        public class TreeNode
        {
            public Dictionary<string, string> Product { get; set; }
            public TreeNode Left { get; set; }
            public TreeNode Right { get; set; }

            public TreeNode(Dictionary<string, string> product)
            {
                Product = product;
                Left = null;
                Right = null;
            }
        }

        private TreeNode root;
        private static List<Dictionary<string, string>> _cachedProducts;
        private static readonly object _cacheLock = new object();

        public Products()
        {
            root = null;
        }

        private DateTime lastCacheRefreshTime = DateTime.MinValue;

        private readonly TimeSpan cacheRefreshInterval = TimeSpan.FromMinutes(30);

        private bool NeedsCacheRefresh()
        {
            return DateTime.Now - lastCacheRefreshTime > cacheRefreshInterval;
        }

      public IEnumerable<Dictionary<string, string>> LoadProductsFromDatabase(List<int> categoryIds, string search)
{
    // Verificar si los datos están en caché
    bool verificacionCache = _cachedProducts == null || _cachedProducts.Count == 0 || NeedsCacheRefresh();
    if (verificacionCache)
    {
        lock (_cacheLock)
        {
            if (verificacionCache)
            {
                // Consultar a la base de datos solo una vez
                List<string[]> productData = StoreDB.RetrieveDatabaseInfo();
                
                // Limpiar el árbol antes de insertar nuevos productos
                root = null;

                // Limpiar la caché antes de insertar nuevos productos
                _cachedProducts = new List<Dictionary<string, string>>();

                // Convertir las filas a diccionarios y validar
                foreach (string[] row in productData)
                {
                    if (row == null || row.Length < 6)
                    {
                        continue; // Ignorar filas inválidas
                    }

                    var productDict = CreateProductDictionary(row);

                    if (ValidateProductDictionary(productDict))
                    {
                        Insert(productDict); // Insertar en el árbol
                        _cachedProducts.Add(productDict); // Agregar a la caché
                    }
                }

                lastCacheRefreshTime = DateTime.Now; // Actualizar el tiempo de actualización de la caché
            }
        }
    }

    // Filtrar los productos en base a categoryIds y search
    IEnumerable<Dictionary<string, string>> result = _cachedProducts;
    bool searchNullCase = !string.IsNullOrWhiteSpace(search) && !search.Equals("null", StringComparison.OrdinalIgnoreCase);
    if (searchNullCase)
    {
        result = SearchProductsByKeyword(root, search);
    }
    bool SearchCategory = categoryIds != null && categoryIds.Count > 0;
    if (SearchCategory)
    {
        // Crear una lista para almacenar los resultados filtrados por categoría
        List<Dictionary<string, string>> categoryFilteredProducts = new List<Dictionary<string, string>>();

        // Filtrar productos por categoría utilizando el árbol binario de búsqueda
        SearchByCategories(root, categoryIds, categoryFilteredProducts);

        // Intersección de los resultados de búsqueda por categoría y los productos filtrados por búsqueda
        result = result.Intersect(categoryFilteredProducts, new DictionaryEqualityComparer());
    }

    return result;
}

private void SearchByCategories(TreeNode node, List<int> categoryIds, List<Dictionary<string, string>> result)
{
    if (result == null)
    {
        throw new ArgumentNullException(nameof(result), "Result list cannot be null.");
    }

    if (node == null)
    {
        return;
    }

    if (!node.Product.ContainsKey("categoryId"))
    {
        throw new ArgumentException("Product dictionary does not contain 'categoryId' key.");
    }

    int currentCategoryId;
    if (!int.TryParse(node.Product["categoryId"], out currentCategoryId))
    {
        throw new ArgumentException("Invalid category ID in product dictionary.");
    }

    // Verificar si el nodo actual pertenece a alguna de las categorías especificadas
    if (categoryIds.Contains(currentCategoryId))
    {
        // Agregar el producto del nodo al resultado
        result.Add(node.Product);
    }

    // Buscar en las ramas izquierda y derecha del árbol
    SearchByCategories(node.Left, categoryIds, result);
    SearchByCategories(node.Right, categoryIds, result);
}

        public void Insert(Dictionary<string, string> product)
        {
            if (root == null)
            {
                root = new TreeNode(product);
            }
            else
            {
                InsertRecursively(root, product);
            }
        }

        private void InsertRecursively(TreeNode node, Dictionary<string, string> product)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node), "El nodo no puede ser nulo.");

            if (product == null)
                throw new ArgumentNullException(nameof(product), "El producto no puede ser nulo.");

            if (!node.Product.ContainsKey("categoryId") || !product.ContainsKey("categoryId"))
                throw new ArgumentException("El diccionario de producto debe contener la clave 'categoryId'.");

            if (!int.TryParse(node.Product["categoryId"], out int currentCategoryId))
                throw new ArgumentException("El valor de 'categoryId' en el nodo no es un número entero válido.");

            if (!int.TryParse(product["categoryId"], out int newCategoryId))
                throw new ArgumentException("El valor de 'categoryId' en el producto no es un número entero válido.");

            if (newCategoryId < currentCategoryId)
            {
                if (node.Left == null)
                {
                    node.Left = new TreeNode(product);
                }
                else
                {
                    InsertRecursively(node.Left, product);
                }
            }
            else
            {
                if (node.Right == null)
                {
                    node.Right = new TreeNode(product);
                }
                else
                {
                    InsertRecursively(node.Right, product);
                }
            }
        }


        public IEnumerable<Dictionary<string, string>> SearchProductsByKeyword(TreeNode node, string keyword)
        {
            // Verifica si el nodo es nulo
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node), "El nodo no puede ser nulo.");
            }

            // Verifica si la palabra clave es nula o está vacía
            if (string.IsNullOrEmpty(keyword))
            {
                throw new ArgumentException("La palabra clave no puede ser nula o vacía.", nameof(keyword));
            }

            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            SearchByKeyword(node, keyword, result);

            return result;
        }

        private void SearchByKeyword(TreeNode node, string keyword, List<Dictionary<string, string>> result)
        {
            // Verifica si el nodo es nulo
            if (node == null)
            {
                return;
            }

            SearchByKeyword(node.Left, keyword, result);

            // Verifica si el producto del nodo contiene la palabra clave
            if (ProductContainsKeyword(node.Product, keyword))
            {
                result.Add(node.Product);
            }

            SearchByKeyword(node.Right, keyword, result);
        }


        public bool ProductContainsKeyword(Dictionary<string, string> product, string keyword)
        {
            // Verifica si el diccionario del producto es nulo
            if (product is null)
                throw new ArgumentNullException(nameof(product), "El diccionario del producto no puede ser nulo.");

            // Verifica si la palabra clave es nula o está vacía
            if (string.IsNullOrEmpty(keyword))
                throw new ArgumentException("La palabra clave no puede ser nula o vacía.", nameof(keyword));

            // Convierte la palabra clave a minúsculas
            keyword = keyword.ToLower();

            // Verificar si alguna propiedad del producto contiene el keyword
            return product.Values.Any(v => v != null && v.ToLower().Contains(keyword));
        }


        public class DictionaryEqualityComparer : IEqualityComparer<Dictionary<string, string>>
        {
            public bool Equals(Dictionary<string, string> x, Dictionary<string, string> y)
            {
                // Verifica si las referencias son iguales
                if (ReferenceEquals(x, y))
                    return true;

                // Verifica si alguno de los diccionarios es nulo
                if (x is null || y is null)
                    return false;

                // Verifica si los diccionarios tienen la misma cantidad de elementos
                if (x.Count != y.Count)
                    return false;

                // Verifica cada par de clave-valor en el primer diccionario
                foreach (var pair in x)
                {
                    // Intenta obtener el valor correspondiente en el segundo diccionario
                    if (!y.TryGetValue(pair.Key, out var value) || value != pair.Value)
                        return false;
                }

                // Si pasa todas las verificaciones anteriores, los diccionarios son iguales
                return true;
            }


            public int GetHashCode(Dictionary<string, string> obj)
            {
                // Verificar si el diccionario es nulo
                if (obj == null)
                {
                    throw new ArgumentNullException(nameof(obj), "Dictionary cannot be null.");
                }

                unchecked
                {
                    int hash = 17;
                    foreach (var pair in obj)
                    {
                        // Verificar si la clave es nula
                        if (pair.Key == null)
                        {
                            throw new ArgumentException("Dictionary key cannot be null.");
                        }

                        // Verificar si el valor es nulo
                        if (pair.Value == null)
                        {
                            throw new ArgumentException("Dictionary value cannot be null.");
                        }

                        hash = hash * 23 + pair.Key.GetHashCode();
                        hash = hash * 23 + pair.Value.GetHashCode();
                    }
                    return hash;
                }
            }
        }
        private bool ValidateProductDictionary(Dictionary<string, string> productDict)
        {
            // Validación de diccionario nulo o insuficiente
            if (productDict == null || productDict.Count < 6)
            {
                throw new ArgumentException("Invalid product data dictionary: Insufficient data.");
            }

            int productId;
            if (!int.TryParse(productDict["id"], out productId) || productId <= 0)
            {
                throw new ArgumentException("Invalid or missing product ID.");
            }

            if (string.IsNullOrWhiteSpace(productDict["name"]))
            {
                throw new ArgumentException("Product name is null or empty.");
            }

            if (string.IsNullOrWhiteSpace(productDict["imageUrl"]))
            {
                throw new ArgumentException("Image URL is null or empty.");
            }

            if (string.IsNullOrWhiteSpace(productDict["description"]))
            {
                throw new ArgumentException("Product description is null or empty.");
            }

            int categoryId;
            if (!int.TryParse(productDict["categoryId"], out categoryId) || categoryId <= 0)
            {
                throw new ArgumentException("Invalid or missing category ID.");
            }

            return true;
        }
        private Dictionary<string, string> CreateProductDictionary(string[] row)
        {
            // Validación de longitud de fila
            if (row.Length < 6)
            {
                throw new ArgumentException("Row data is insufficient.");
            }

            int id;
            if (!int.TryParse(row[0], out id) || id <= 0)
            {
                throw new ArgumentException("Invalid or missing product ID.");
            }

            string name = row[1];
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product name is null or empty.");
            }

            decimal price;
            if (!decimal.TryParse(row[2], out price) || price < 0)
            {
                throw new ArgumentException("Invalid product price.");
            }

            string imageUrl = row[3];
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                throw new ArgumentException("Image URL is null or empty.");
            }

            string description = row[4];
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Product description is null or empty.");
            }

            int categoryId;
            if (!int.TryParse(row[5], out categoryId) || categoryId <= 0)
            {
                throw new ArgumentException("Invalid or missing category ID.");
            }

            return new Dictionary<string, string>
            {
                { "id", id.ToString() },
                { "name", name },
                { "price", price.ToString("0.00", CultureInfo.InvariantCulture) },           
                { "imageUrl", imageUrl },
                { "description", description },
                { "categoryId", categoryId.ToString() }
            };
        }

    }
}


