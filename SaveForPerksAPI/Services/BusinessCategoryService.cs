using AutoMapper;
using SaveForPerksAPI.Common;
using SaveForPerksAPI.Models;
using SaveForPerksAPI.Repositories;

namespace SaveForPerksAPI.Services;

public class BusinessCategoryService : IBusinessCategoryService
{
    private readonly ISaveForPerksRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<BusinessCategoryService> _logger;

    public BusinessCategoryService(
        ISaveForPerksRepository repository,
        IMapper mapper,
        ILogger<BusinessCategoryService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<IEnumerable<BusinessCategoryDto>>> GetAllCategoriesAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all business categories");

            var categories = await _repository.GetAllBusinessCategoriesAsync();

            var categoryDtos = _mapper.Map<IEnumerable<BusinessCategoryDto>>(categories);

            _logger.LogInformation(
                "Retrieved {Count} reward owner categories",
                categoryDtos.Count());

            return Result<IEnumerable<BusinessCategoryDto>>.Success(categoryDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to retrieve reward owner categories. Error: {Error}",
                ex.Message);
            return Result<IEnumerable<BusinessCategoryDto>>.Failure(
                "An error occurred while retrieving categories");
        }
    }
}
