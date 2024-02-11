using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Application.Validators.Athlete;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Utilities.Static;
using BC = BCrypt.Net.BCrypt;

namespace Application.Services
{
    public class AthleteApplication : IAthleteApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AthleteValidator _validationRules;
        private readonly DbFithubContext _context;
        private readonly IJwtHandler _jwtHandler;

        public AthleteApplication(IUnitOfWork unitOfWork, IMapper mapper, AthleteValidator validationRules, DbFithubContext _context, IJwtHandler jwtHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validationRules = validationRules;
            this._context = _context;
            _jwtHandler = jwtHandler;
        }

        public async Task<bool> AccessAthlete(string accessAthleteDto)
        {
            bool response = false;
            IDbContextTransaction? transaction = null;

            try
            {
                var existCardAccess = await _unitOfWork.CardAccessRepository.GetAccessCardByCode(accessAthleteDto);

                if (existCardAccess is null || existCardAccess.Status == false)
                {
                    throw new Exception("El código de acceso no existe");
                }

                var athlete = await AthleteById(existCardAccess.IdAthlete);

                if (athlete.Data is null)
                {
                    throw new Exception("El atleta no existe");
                }

                if (athlete.Data.EndDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    throw new Exception("El atleta no cuenta con una membresía activa");
                }

                transaction = _context.Database.BeginTransaction();

                var access = new AccessLog
                {
                    IdAthlete = athlete.Data.AthleteId,
                    IdCard = existCardAccess.CardId,
                    AccessDateTime = DateTime.Now,
                    IdGym = athlete.Data.IdGym,
                };

                var resultAccess = await _unitOfWork.AccessLogRepository.RegisterAccessLog(access);

                if (!resultAccess)
                {
                    throw new Exception("Error al registrar el acceso");
                }

                transaction.Commit();

                response = true;
            }
            catch (Exception ex)
            {
                response = false;
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<AthleteResponseDto>> AthleteById(int athleteID)
        {
            var response = new BaseResponse<AthleteResponseDto>();
            var athlete = await _unitOfWork.AthleteRepository.AthleteById(athleteID);

            if (athlete is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<AthleteResponseDto>(athlete);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EditAthlete(int athleteID, AthleteRequestDto athleteDto)
        {
            var response = new BaseResponse<bool>();
            IDbContextTransaction? transaction = null;

            try
            {
                var athleteEdit = await AthleteById(athleteID);

                if (athleteEdit.Data is null)
                {
                    throw new Exception("El atleta no existe");
                }

                transaction = _context.Database.BeginTransaction();

                var athlete = _mapper.Map<Athlete>(athleteDto);
                athlete.AthleteId = athleteID;
                athlete.AuditUpdateDate = DateTime.Now;
                athlete.AuditUpdateUser = athleteDto.GymName;
                response.Data = await _unitOfWork.AthleteRepository.EditAthlete(athlete);

                if (!response.Data)
                {
                    throw new Exception("Error al editar al atleta");
                }

                var existCardAccess = await _unitOfWork.CardAccessRepository.GetAccessCardByCode(athleteEdit.Data.CardAccessCode);

                if (existCardAccess != null && existCardAccess.Status == true && existCardAccess.IdAthlete != athleteID)
                {
                    throw new Exception("El código de acceso ya se encuentra registrado");
                }

                var existNewCardAccess = await _unitOfWork.CardAccessRepository.GetAccessCardByCode(athleteDto.CardAccessCode);

                if (existNewCardAccess != null && existNewCardAccess.Status == true && existNewCardAccess.IdAthlete != athleteID)
                {
                    throw new Exception("El código de acceso ya se encuentra registrado");
                }

                if (existCardAccess != null && existCardAccess.CardNumber != athleteDto.CardAccessCode)
                {
                    var updateCardAccess = existCardAccess;
                    updateCardAccess.Status = false;

                    var resultUpdateCardAccess = await _unitOfWork.CardAccessRepository.UnregisterCardAccess(updateCardAccess);

                    if (!resultUpdateCardAccess)
                    {
                        throw new Exception("Error al actualizar el código de acceso");
                    }

                    var newCardAccess = new CardAccess
                    {
                        IdAthlete = athlete.AthleteId,
                        CardNumber = athleteDto.CardAccessCode,
                        ExpirationDate = null,
                        Status = true,
                    };

                    var resultCardAccess = await _unitOfWork.CardAccessRepository.RegisterCardAccess(newCardAccess);
                    if (!resultCardAccess)
                    {
                        throw new Exception("Error al registrar el código de acceso");
                    }
                }

                if (existCardAccess is null)
                {
                    var newCardAccess = new CardAccess
                    {
                        IdAthlete = athlete.AthleteId,
                        CardNumber = athleteDto.CardAccessCode,
                        ExpirationDate = null,
                        Status = true,
                    };

                    var resultCardAccess = await _unitOfWork.CardAccessRepository.RegisterCardAccess(newCardAccess);
                    if (!resultCardAccess)
                    {
                        throw new Exception("Error al registrar el código de acceso");
                    }
                }

                transaction.Commit();

                response.IsSuccess = true;
                response.Data = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<BaseEntityResponse<AthleteResponseDto>>> ListAthletes(BaseFiltersRequest filters)
        {
            var gymID = _jwtHandler.ExtractGymIdFromToken();
            var response = new BaseResponse<BaseEntityResponse<AthleteResponseDto>>();
            var athletes = await _unitOfWork.AthleteRepository.ListAthlete(filters, gymID);

            if (athletes is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<BaseEntityResponse<AthleteResponseDto>>(athletes);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<AthleteResponseDto>> LoginAthlete(LoginRequestDto loginDto)
        {
            var response = new BaseResponse<AthleteResponseDto>();
            var athlete = await _unitOfWork.AthleteRepository.LoginAthlete(loginDto.Email);

            if (athlete is not null)
            {
                if (BC.Verify(loginDto.Password, athlete.Password))
                {
                    response.IsSuccess = true;
                    response.Data = _mapper.Map<AthleteResponseDto>(athlete);
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterAthlete(AthleteRequestDto athleteDto)
        {
            var response = new BaseResponse<bool>();
            IDbContextTransaction? transaction = null;

            try
            {
                var validationResults = await _validationRules.ValidateAsync(athleteDto);

                if (!validationResults.IsValid)
                {
                    response.IsSuccess = false;
                    response.Errors = validationResults.Errors;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    throw new Exception("Error al validar los datos");
                }

                transaction = _context.Database.BeginTransaction();
                var athlete = _mapper.Map<Athlete>(athleteDto);
                athlete.AuditCreateDate = DateTime.Now;
                athlete.AuditCreateUser = athleteDto.GymName;

                var result = await _unitOfWork.AthleteRepository.RegisterAthlete(athlete);

                if (!result)
                {
                    throw new Exception("Error al registrar al atleta");
                }

                var membershiptDuration = await _unitOfWork.MembershipRepository.GetMembershipById(athleteDto.MembershipId);
                var athleteMembership = new AthleteMembership
                {
                    IdAthlete = athlete.AthleteId,
                    IdMembership = athleteDto.MembershipId,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(membershiptDuration.DurationInDays)),
                };

                var resultAthleteMembership = await _unitOfWork.AthleteMembershipRepository.RegisterAthleteMembership(athleteMembership);
                if (!resultAthleteMembership)
                {
                    throw new Exception("Error al registrar la membresía del atleta");
                }

                if (athleteDto.CardAccessCode is null)
                {
                    throw new Exception("El código de acceso no puede ser nulo");
                }

                var existCardAccess = await _unitOfWork.CardAccessRepository.GetActiveAccessByCode(athleteDto.CardAccessCode);

                if (existCardAccess)
                {
                    throw new Exception("El código de acceso ya se encuentra registrado");
                }

                var cardAccess = new CardAccess
                {
                    IdAthlete = athlete.AthleteId,
                    CardNumber = athleteDto.CardAccessCode,
                    ExpirationDate = null,
                    Status = true,
                };

                var resultCardAccess = await _unitOfWork.CardAccessRepository.RegisterCardAccess(cardAccess);
                if (!resultCardAccess)
                {
                    throw new Exception("Error al registrar el código de acceso");
                }

                transaction.Commit();

                response.IsSuccess = true;
                response.Data = result;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RemoveAthlete(int athleteID)
        {
            var response = new BaseResponse<bool>();
            var athlete = await AthleteById(athleteID);

            if (athlete.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            response.Data = await _unitOfWork.AthleteRepository.DeleteAthlete(athleteID);

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_DELETE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> UpdateMembershipToAthlete(MembershipToAthleteRequestDto membershipToAthleteDto)
        {
            var response = new BaseResponse<bool>();
            IDbContextTransaction? transaction = null;

            try
            {
                var athleteEdit = await AthleteById(membershipToAthleteDto.AthleteId);

                if (athleteEdit.Data is null)
                {
                    throw new Exception("El atleta no existe");
                }

                if (athleteEdit.Data.EndDate > DateOnly.FromDateTime(DateTime.Now))
                {
                    throw new Exception("El atleta ya cuenta con una membresía activa");
                }

                transaction = _context.Database.BeginTransaction();

                var membershiptDuration = await _unitOfWork.MembershipRepository.GetMembershipById(membershipToAthleteDto.MembershipId);
                var athleteMembership = new AthleteMembership
                {
                    IdAthlete = membershipToAthleteDto.AthleteId,
                    IdMembership = membershipToAthleteDto.MembershipId,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(membershiptDuration.DurationInDays)),
                };

                var resultAthleteMembership = await _unitOfWork.AthleteMembershipRepository.RegisterAthleteMembership(athleteMembership);
                if (!resultAthleteMembership)
                {
                    throw new Exception("Error al registrar la membresía del atleta");
                }

                transaction.Commit();

                response.IsSuccess = true;
                response.Data = resultAthleteMembership;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }
    }
}
