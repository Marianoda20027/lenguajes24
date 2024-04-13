using MediatR;
using StoreApi.Models;
using StoreApi.Queries;
using StoreApi.Repositories;

namespace StoreApi.Handler
{
    public class GetProductByIdHandler :  IRequestHandler<GetProductByIdQuery, Product>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductByIdAsync(query.Uuid);
        }
    }
}
