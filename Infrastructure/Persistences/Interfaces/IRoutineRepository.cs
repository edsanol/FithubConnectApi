﻿using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IRoutineRepository
    {
        Task<bool> CreateRoutine(Routines routine);
        Task<BaseEntityResponse<Routines>> GetRoutinesListByGymId(BaseFiltersRequest filters, int gymId);
    }
}
