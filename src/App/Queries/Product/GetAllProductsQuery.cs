using MediatR;
using Dom.Entities;
using System.Collections.Generic;
using App.DTOs;

namespace App.Queries.Product;

public record GetAllProductsQuery() : IRequest<IEnumerable<ProductDto>>;

