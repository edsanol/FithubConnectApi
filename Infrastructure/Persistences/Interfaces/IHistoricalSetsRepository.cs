﻿using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IHistoricalSetsRepository
    {
        Task<bool> InsertHistoricalSets(List<HistoricalSets> historicalSets);
    }
}
