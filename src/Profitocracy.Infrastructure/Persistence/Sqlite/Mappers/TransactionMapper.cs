using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Domain.Model.Transactions.Entities;
using Profitocracy.Core.Domain.Model.Transactions.Factories;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Infrastructure.Abstractions.Internal;
using Profitocracy.Infrastructure.Persistence.Sqlite.Models.Transaction;

namespace Profitocracy.Infrastructure.Persistence.Sqlite.Mappers;

internal class TransactionMapper : IInfrastructureMapper<Transaction, TransactionModel>
{
	public Transaction MapToDomain(TransactionModel model)
	{
		TransactionGeoTag? geoTag = null;
		TransactionCategory? category = null;
		
		if (model.GeoTagLongitude is not null && model.GeoTagLatitude is not null)
		{
			geoTag = new TransactionGeoTag
			{
				Latitude = (double)model.GeoTagLatitude,
				Longitude = (double)model.GeoTagLongitude
			};
		}

		if (model.CategoryId is not null && model.CategoryName is not null)
		{
			category = new TransactionCategory((Guid)model.CategoryId)
			{
				Name = model.CategoryName
			};
		}
		
		if (model.DestinationCurrencyCode is not null)
		{
			// The model is supposed to be a multi currency transaction,
			// so we suppress warnings related to null reference with
			// null-forgiving operator (!)
			return TransactionFactory.CreateMultiCurrencyTransaction(
				model.Id,
				model.Amount,
				(decimal)model.DestinationAmount!,
				Currency.AvailableCurrencies.All[model.SourceCurrencyCode!],
				Currency.AvailableCurrencies.All[model.DestinationCurrencyCode],
				model.ProfileId,
				(TransactionType)model.Type,
				model.SpendingType is null ? null : (SpendingType)model.SpendingType,
				(TransactionDestination)model.Destination!,
				model.Timestamp,
				model.Description,
				geoTag,
				category);
		}
		
		return TransactionFactory.CreateTransaction(
			model.Id,
			model.Amount,
			model.ProfileId,
			(TransactionType)model.Type,
			model.SpendingType is null ? null : (SpendingType)model.SpendingType,
			model.Timestamp,
			model.Description,
			geoTag,
			category);
	}

	public TransactionModel MapToModel(Transaction entity)
	{
		var model = new TransactionModel
		{
			Id = entity.Id,
			Amount = entity.Amount,
			ProfileId = entity.ProfileId,
			Type = (short)entity.Type,
			SpendingType = entity.SpendingType is null ? null : (short)entity.SpendingType,
			Timestamp = entity.Timestamp,
			Description = entity.Description,
			GeoTagLatitude = entity.GeoTag?.Latitude,
			GeoTagLongitude = entity.GeoTag?.Longitude,
			CategoryId = entity.Category?.Id,
			CategoryName = entity.Category?.Name
		};

		if (entity is MultiCurrencyTransaction multiTransaction)
		{
			model.Destination = (short)multiTransaction.Destination;
			model.DestinationAmount = multiTransaction.DestinationAmount;
			model.SourceCurrencyCode = multiTransaction.SourceCurrency.Code;
			model.DestinationCurrencyCode = multiTransaction.DestinationCurrency.Code;
		}

		return model;
	}
}