using MediatR;
using System;

namespace App.Commands.Product;

public record UpdateProductCommand(Guid Id, string Name, string Description, decimal Price, int Stock)
    : IRequest<Unit>;
