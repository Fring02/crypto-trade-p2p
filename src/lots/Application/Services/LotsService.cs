using System.Linq.Expressions;
using System.Net;
using Application.Services.Base;
using AutoMapper;
using Data.Extensions;
using Domain.Enums;
using Domain.Models;
using Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using Shared.Dtos;
using Shared.Interfaces.Repositories;
using Shared.Interfaces.Services;
namespace Application.Services;

public class LotsService : CrudService<LotCreateDto, LotUpdateDto, LotItemDto, LotViewDto, ILotsRepository, Lot, long>, ILotsService
{
    private readonly RestClient _client;
    private readonly string _requisitesUrl;
    private readonly ILogger<LotsService> _logger;
    public LotsService(ILotsRepository repository, IMapper mapper, RestClient client, IOptions<ConnectionStrings> options, ILogger<LotsService> logger) : base(repository, mapper)
    {
        _client = client;
        _logger = logger;
        _requisitesUrl = options.Value.RequisitesUrl;
    }

    public override async Task<Lot> CreateAsync(LotCreateDto entity, CancellationToken token = default)
    {
        var cryptoType = Enum.Parse<CryptoCurrency>(entity.CryptoType);
        var fiatType = Enum.Parse<FiatCurrency>(entity.FiatType);
        if (await Repository.ExistsAsync(e => e.OwnerEmail == entity.OwnerEmail && e.OwnerWallet == entity.OwnerWallet && e.CryptoType == cryptoType && e.FiatType == fiatType, token))
            throw new ArgumentException("Lot with these details already exists.");
        var details = Mapper.Map<Lot>(entity);
        details.CreatedAt = DateTime.UtcNow;
        return await Repository.CreateAsync(details, token);
    }

    public override async Task<Lot> UpdateAsync(LotUpdateDto entity, CancellationToken token = default)
    {
        var lot = await Repository.FindOneAsync(l => l.Id == entity.Id, token);
        if (lot is null) throw new ArgumentException($"Lot with id {entity.Id} is not found");
        if (entity.RequisiteId != null) lot.RequisiteId = entity.RequisiteId.Value;
        if (!string.IsNullOrEmpty(entity.CryptoType)) lot.CryptoType = Enum.Parse<CryptoCurrency>(entity.CryptoType);
        if (!string.IsNullOrEmpty(entity.FiatType)) lot.FiatType = Enum.Parse<FiatCurrency>(entity.FiatType);
        if (!string.IsNullOrEmpty(entity.LotType)) lot.Type = Enum.Parse<LotType>(entity.LotType);
        if (entity.Price is > 0) lot.Price = entity.Price.Value;
        if (entity.Supply is > 0) lot.Supply = entity.Supply.Value;
        if (entity.MinLimit is > 0)
        {
            if (entity.MinLimit > lot.MaxLimit) throw new ArgumentException("Minimum limit must not exceed maximum limit");
            lot.MinLimit = entity.MinLimit.Value;
        }
        if (entity.MaxLimit is > 0)
        {
            if (entity.MaxLimit < lot.MinLimit) throw new ArgumentException("Maximum limit must be greater than minimum limit");
            lot.MaxLimit = entity.MaxLimit.Value;
        }
        lot = await Repository.UpdateAsync(lot, token);
        return lot;
    }

    public async Task<(int, ICollection<LotItemDto>)> GetAllAsync(LotFilterDto? filter = null, int page = 0, int pageCount = 0, CancellationToken token = default)
    {
        if (filter == null)
            return await Repository.GetAll<LotItemDto>(page: page, pageCount: pageCount, token: token);
        
        var filterExpr = PredicateBuilder.True<Lot>();
        if (!string.IsNullOrEmpty(filter.CryptoType))
        {
            var cryptoType = Enum.Parse<CryptoCurrency>(filter.CryptoType);
            filterExpr = filterExpr.And(f => f.CryptoType == cryptoType);
        }
        if (!string.IsNullOrEmpty(filter.FiatType))
        {
            var fiatType = Enum.Parse<FiatCurrency>(filter.FiatType);
            filterExpr = filterExpr.And(f => f.FiatType == fiatType);
        }
        if (!string.IsNullOrEmpty(filter.LotType))
        {
            var lotType = Enum.Parse<LotType>(filter.LotType);
            filterExpr = filterExpr.And(f => f.Type == lotType);
        }
        if (filter.MinLimit > 0 && filter.MaxLimit > 0) filterExpr = filterExpr.And(f => f.MinLimit >= filter.MinLimit && f.MaxLimit <= filter.MaxLimit);
        if (filter.MinPrice > 0 && filter.MaxPrice > 0) filterExpr = filterExpr.And(f => f.Price >= filter.MinPrice && f.Price <= filter.MaxPrice);
        if (filter.MinSupply > 0 && filter.MaxSupply > 0) filterExpr = filterExpr.And(f => f.Supply >= filter.MinSupply && f.Supply <= filter.MaxSupply);
        return await Repository.GetAll<LotItemDto>(filterExpr, page, pageCount, token);
    }

    public async Task<LotViewDto?> GetByIdAsync(long id, string? accessToken = null, CancellationToken token = default)
    {
        var lot = await GetByIdAsync(id, token);
        if (lot is null) throw new ArgumentException($"Lot with id {id} is not found");
        _logger.LogInformation("Found lot with details: Id = {Id}, Owner = {OwnerEmail}, Wallet = {OwnerWallet}", id, lot.OwnerEmail, lot.OwnerWallet);
        string url = _requisitesUrl + $"/{lot.RequisiteId}";
        var request = new RestRequest(_requisitesUrl + $"/{lot.RequisiteId}");
        _logger.LogInformation("Started building HTTP request for {Url}", url);
        request.AddOrUpdateHeader("Authorization", $"Bearer {accessToken}");
        _logger.LogInformation("Appended header => Authorization: Bearer {AccessToken}", accessToken);
        var response = await _client.GetAsync<RequisiteDto>(request, token);
        if (response is null) throw new HttpRequestException("Failed to fetch requisite");
        lot.Requisite = response;
        return lot;
    }
}