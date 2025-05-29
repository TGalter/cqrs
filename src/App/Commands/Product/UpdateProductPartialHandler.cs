using Dom.Interfaces;
using MediatR;
using System; 
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace App.Commands.Product
{
    public class UpdateProductPartialHandler : IRequestHandler<UpdateProductPartialCommand,Unit>
    {
        private readonly IProductRepository _repository;

        public UpdateProductPartialHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateProductPartialCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.Id);
            if (product is null)
                throw new KeyNotFoundException("Produto não encontrado.");

            if (!string.IsNullOrWhiteSpace(request.Name))
                product.SetName(request.Name);

            if (request.Description is not null)
                product.SetDescription(request.Description);

            if (request.Price.HasValue)
                product.SetPrice(request.Price.Value);

            if (request.Stock.HasValue)
            {
                int diff = request.Stock.Value - product.Stock;
                if (diff > 0)
                    product.AddStock(diff);
                else if (diff < 0)
                    product.RemoveStock(Math.Abs(diff));
            }

            await _repository.UpdateAsync(product);
            return Unit.Value;
        }
    }
}
