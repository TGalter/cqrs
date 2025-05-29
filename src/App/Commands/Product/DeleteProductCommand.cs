using MediatR;
using System;

namespace App.Commands.Product;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;
