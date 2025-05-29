using MediatR;
using App.DTOs;
using Dom.Interfaces;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using App.Interfaces;

namespace App.Queries.Product
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IElasticSearchService _elasticSearchService;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IElasticSearchService elasticSearchService, IMapper mapper)
        {
            _elasticSearchService = elasticSearchService;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _elasticSearchService.GetProductByIdAsync(request.Id);

            return _mapper.Map<ProductDto>(product);
        }
    }
}

