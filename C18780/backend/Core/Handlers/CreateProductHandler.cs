using MediatR;
using StoreApi.Commands;
using StoreApi.Models;
using StoreApi.Repositories;

namespace StoreApi.Handler
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Product>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            ValidateCommand(command);

            var product = new Product()
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                ImageUrl = command.ImageUrl
            };

            return await _productRepository.AddProductAsync(product);
        }

        private void ValidateCommand(CreateProductCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                throw new ArgumentException("The name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(command.Description))
            {
                throw new ArgumentException("The description cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(command.ImageUrl))
            {
                throw new ArgumentException("The picture cannot be empty.");
            }
            if (command.Price <= 0)
            {
                throw new ArgumentException("The price must be greater than zero.");
            }
        }
    }
}