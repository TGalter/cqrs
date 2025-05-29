using MediatR;
using Dom.Entities;
using Dom.Interfaces;

namespace App.Commands.Product;
public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repository;

    public DeleteProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);

        if (product is null)
            return false;

        await _repository.DeleteAsync(product);

        return true;
    }
}
