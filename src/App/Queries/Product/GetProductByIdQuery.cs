using MediatR;
using App.DTOs;
using System;

namespace App.Queries.Product;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
