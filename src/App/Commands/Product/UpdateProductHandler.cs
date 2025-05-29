using Dom.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace App.Commands.Product
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository _repository;

        public UpdateProductHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.Id);

            if (product is null)
                throw new KeyNotFoundException("Produto não encontrado.");

            product.SetName(request.Name);
            product.SetDescription(request.Description);
            product.SetPrice(request.Price);
            product.SetStock(request.Stock);

            await _repository.UpdateAsync(product);

            return Unit.Value;
        }
    }
}
