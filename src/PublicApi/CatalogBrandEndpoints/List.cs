using Ardalis.ApiEndpoints;
using AutoMapper;
using BlazorShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.PublicApi.CatalogBrandEndpoints
{
    public class List : BaseAsyncEndpoint
        .WithoutRequest
        .WithResponse<ListCatalogBrandsResponse>
    {
        private readonly IAsyncRepository<CatalogBrand> _catalogBrandRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<List> _logger;

        public List(IAsyncRepository<CatalogBrand> catalogBrandRepository,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<List> logger)
        {
            _catalogBrandRepository = catalogBrandRepository;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("api/catalog-brands")]
        [SwaggerOperation(
            Summary = "List Catalog Brands",
            Description = "List Catalog Brands",
            OperationId = "catalog-brands.List",
            Tags = new[] { "CatalogBrandEndpoints" })
        ]
        public override async Task<ActionResult<ListCatalogBrandsResponse>> HandleAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("AAAAAAAAAAAA " + _configuration.GetValue<BaseUrlConfiguration>("baseUrls").WebBase);
            var response = new ListCatalogBrandsResponse();

            var items = await _catalogBrandRepository.ListAllAsync(cancellationToken);

            response.CatalogBrands.AddRange(items.Select(_mapper.Map<CatalogBrandDto>));

            return Ok(response);
        }
    }
}
