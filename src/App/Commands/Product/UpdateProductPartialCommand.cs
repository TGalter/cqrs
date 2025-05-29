using MediatR;
using System;

namespace App.Commands.Product;

public class UpdateProductPartialCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
}
