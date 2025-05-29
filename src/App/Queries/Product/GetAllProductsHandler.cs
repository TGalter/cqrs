using App.DTOs;
using App.Interfaces;
using AutoMapper;
using Dom.Entities;
using Dom.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace App.Queries.Product
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IElasticSearchService _elasticSearchService;
        private readonly IMapper _mapper;

        public GetAllProductsHandler(IElasticSearchService elasticSearchService, IMapper mapper)
        {
            _elasticSearchService = elasticSearchService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var response = await _elasticSearchService.GetAllProductsAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(response);
        }
    }
}

