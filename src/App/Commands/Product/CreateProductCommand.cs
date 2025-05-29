using MediatR;

namespace App.Commands.Product;

public record CreateProductCommand(string Name, string Description, decimal Price, int Stock)
    : IRequest<Guid>;
