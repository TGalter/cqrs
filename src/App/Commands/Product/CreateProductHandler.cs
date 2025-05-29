using Dom.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace App.Commands.Product
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _repository;

        public CreateProductHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Dom.Entities.Product(request.Name, request.Description, request.Price, request.Stock);
            await _repository.AddAsync(product);
            return product.Id;
        }
    }
}
