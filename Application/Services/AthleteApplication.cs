using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Application.Validators.Athlete;
using Application.Validators.MeasurementProgress;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Helpers;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.Xml;
using System.Text;
using Utilities.Static;
using BC = BCrypt.Net.BCrypt;

namespace Application.Services
{
    public class AthleteApplication : IAthleteApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AthleteValidator _validationRules;
        private readonly MeasurementProgressValidator _measurementValidationRules;
        private readonly DbFithubContext _context;
        private readonly IJwtHandler _jwtHandler;
        private readonly ICryptographyApplication _cryptographyApplication;
        private readonly IConfiguration _configuration;

        public AthleteApplication(IUnitOfWork unitOfWork,
            IMapper mapper,
            AthleteValidator validationRules,
            MeasurementProgressValidator measurementValidationRules,
            DbFithubContext _context,
            IJwtHandler jwtHandler,
            ICryptographyApplication cryptographyApplication,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validationRules = validationRules;
            _measurementValidationRules = measurementValidationRules;
            this._context = _context;
            _jwtHandler = jwtHandler;
            _cryptographyApplication = cryptographyApplication;
            _configuration = configuration;
        }

        private byte[] GetJwtSecret()
        {
            return Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? _configuration["Jwt:Secret"]!);
        }

        private string GetJwtKey()
        {
            return Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? _configuration["Jwt:Key"]!;
        }

        private async Task<bool> RefreshTokenLogic(int athleteID, string refreshToken, string actualRefreshToken = "")
        {
            if (!string.IsNullOrEmpty(actualRefreshToken) && actualRefreshToken.Length > 0)
            {
                var revokeTokenStatus = await _unitOfWork.AthleteTokenRepository.RevokeAthleteToken(actualRefreshToken);

                if (!revokeTokenStatus)
                {
                    return false;
                }
            }

            var newRefreshToken = new AthleteToken
            {
                TokenID = 0,
                IdAthlete = athleteID,
                Token = refreshToken,
                Expires = DateOnly.FromDateTime(DateTime.Now.AddMinutes(21600)),
                Revoked = false,
            };

            bool result = await _unitOfWork.AthleteTokenRepository.RegisterAthleteToken(newRefreshToken);

            if (!result)
            {
                return false;
            }

            return true;
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
                    AccessType = 1
                };

                var resultAccess = await _unitOfWork.AccessLogRepository.RegisterAccessLog(access);

                if (!resultAccess)
                {
                    throw new Exception("Error al registrar el acceso");
                }

                transaction.Commit();

                response = true;
            }
            catch (Exception)
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
            string role = _jwtHandler.GetRoleFromToken();

            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var gymID = _jwtHandler.ExtractIdFromToken();

            var hasAthlete = await _unitOfWork.GymRepository.HasAthleteByAthleteID(gymID, athleteID);
            if (!hasAthlete)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

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

            string role = _jwtHandler.GetRoleFromToken();

            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var gymID = _jwtHandler.ExtractIdFromToken();

            var hasAthlete = await _unitOfWork.GymRepository.HasAthleteByAthleteID(gymID, athleteID);
            if (!hasAthlete)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

            if (!string.IsNullOrEmpty(athleteDto.Email))
            {
                var emailExists = await _unitOfWork.AthleteRepository.CheckEmailExists(athleteDto.Email, athleteID);
                if (emailExists)
                {
                    response.IsSuccess = false;
                    response.Message = "El correo electrónico ya está en uso.";
                    return response;
                }
            }

            try
            {
                var gym = await _unitOfWork.GymRepository.GetGymById(gymID);
                var athleteEdit = await _unitOfWork.AthleteRepository.AthleteById(athleteID) ?? throw new Exception("El atleta no existe");
                transaction = _context.Database.BeginTransaction();

                var athlete = _mapper.Map<Athlete>(athleteDto);
                athlete.AthleteId = athleteEdit.AthleteId;
                athlete.IdGym = athleteEdit.IdGym;
                athlete.Password = athleteEdit.Password;
                athlete.FingerPrint = athleteEdit.FingerPrint;
                athlete.AuditCreateDate = athleteEdit.AuditCreateDate;
                athlete.AuditCreateUser = athleteEdit.AuditCreateUser;
                athlete.AuditUpdateDate = DateTime.Now;
                athlete.AuditUpdateUser = gym.GymName;
                athlete.Email = athleteDto.Email;

                response.Data = await _unitOfWork.AthleteRepository.EditAthlete(athlete);

                if (!response.Data)
                {
                    throw new Exception("Error al editar al atleta");
                }

                //var existCardAccess = await _unitOfWork.CardAccessRepository.GetAccessCardByCode(athleteEdit.CardAccesses.Last().CardNumber);

                //if (existCardAccess != null && existCardAccess.Status == true && existCardAccess.IdAthlete != athleteID)
                //{
                //    throw new Exception("El código de acceso ya se encuentra registrado");
                //}

                //var existNewCardAccess = await _unitOfWork.CardAccessRepository.GetAccessCardByCode(athleteDto.CardAccessCode);

                //if (existNewCardAccess != null && existNewCardAccess.Status == true && existNewCardAccess.IdAthlete != athleteID)
                //{
                //    throw new Exception("El código de acceso ya se encuentra registrado");
                //}

                //if (existCardAccess != null && existCardAccess.CardNumber != athleteDto.CardAccessCode)
                //{
                //    var updateCardAccess = existCardAccess;
                //    updateCardAccess.Status = false;

                //    var resultUpdateCardAccess = await _unitOfWork.CardAccessRepository.UnregisterCardAccess(updateCardAccess);

                //    if (!resultUpdateCardAccess)
                //    {
                //        throw new Exception("Error al actualizar el código de acceso");
                //    }

                //    var newCardAccess = new CardAccess
                //    {
                //        IdAthlete = athlete.AthleteId,
                //        CardNumber = athleteDto.CardAccessCode,
                //        ExpirationDate = null,
                //        Status = true,
                //    };

                //    var resultCardAccess = await _unitOfWork.CardAccessRepository.RegisterCardAccess(newCardAccess);
                //    if (!resultCardAccess)
                //    {
                //        throw new Exception("Error al registrar el código de acceso");
                //    }
                //}

                //if (existCardAccess is null)
                //{
                //    var newCardAccess = new CardAccess
                //    {
                //        IdAthlete = athlete.AthleteId,
                //        CardNumber = athleteDto.CardAccessCode,
                //        ExpirationDate = null,
                //        Status = true,
                //    };

                //    var resultCardAccess = await _unitOfWork.CardAccessRepository.RegisterCardAccess(newCardAccess);
                //    if (!resultCardAccess)
                //    {
                //        throw new Exception("Error al registrar el código de acceso");
                //    }
                //}

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

        public async Task<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>>
            GetMeasurementsGraphic(string muscle, DateOnly startDate, DateOnly endDate, int athleteID)
        {
            var userID = _jwtHandler.ExtractIdFromToken();
            var response = new BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>();
            string role = _jwtHandler.GetRoleFromToken();

            if (role == "deportista")
            {
                athleteID = userID;
            }
            else if (role == "gimnasio" && athleteID > 0)
            {
                bool hasAthlete = await _unitOfWork.GymRepository.HasAthleteByAthleteID(userID, athleteID);
                if (!hasAthlete)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var graphic = await _unitOfWork.MeasurementProgressRepository.GetMeasurementsGraphic(athleteID, muscle, startDate, endDate);

            response.IsSuccess = true;
            response.Data = _mapper.Map<IEnumerable<DashboardGraphicsResponseDto>>(graphic);
            response.Message = ReplyMessage.MESSAGE_QUERY;

            return response;
        }

        public async Task<BaseResponse<BaseEntityResponse<MeasurementProgressResponseDto>>>
            GetMeasurementProgressList(BaseFiltersRequest filters, int athleteID)
        {
            var response = new BaseResponse<BaseEntityResponse<MeasurementProgressResponseDto>>();
            var userID = _jwtHandler.ExtractIdFromToken();
            string role = _jwtHandler.GetRoleFromToken();

            if (role == "deportista")
            {
                athleteID = userID;
            }
            else if (role == "gimnasio" && athleteID > 0)
            {
                bool hasAthlete = await _unitOfWork.GymRepository.HasAthleteByAthleteID(userID, athleteID);
                if (!hasAthlete)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var measurementProgress = await _unitOfWork.MeasurementProgressRepository.GetMeasurementProgressList(filters, athleteID);

            if (measurementProgress is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<BaseEntityResponse<MeasurementProgressResponseDto>>(measurementProgress);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<BaseEntityResponse<AthleteResponseDto>>> ListAthletes(BaseFiltersRequest filters)
        {
            var gymID = _jwtHandler.ExtractIdFromToken();
            var response = new BaseResponse<BaseEntityResponse<AthleteResponseDto>>();
            var athletes = await _unitOfWork.AthleteRepository.ListAthlete(filters, gymID);

            if (athletes is not null && filters.NumFilter != 5)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<BaseEntityResponse<AthleteResponseDto>>(athletes);
                foreach (var item in response.Data.Items)
                {
                    item.FingerPrint = null;
                }
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else if (athletes is not null && filters.NumFilter == 5)
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
                    response.Data.Token = await _jwtHandler.GenerateAthleteToken(athlete);
                    response.Data.RefreshToken = await _jwtHandler.GenerateAthleteRefreshToken(athlete);
                    response.Message = ReplyMessage.MESSAGE_LOGIN;

                    bool result = await RefreshTokenLogic(athlete.AthleteId, response.Data.RefreshToken);

                    if (!result)
                    {
                        response.IsSuccess = false;
                        response.Message = ReplyMessage.MESSAGE_FAILED;
                        response.Data = null;
                    }
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

        public async Task<BaseResponse<bool>> RecordMeasurementProgress(MeasurementProgressRequestDto measurementProgressDto)
        {
            var response = new BaseResponse<bool>();
            int userID = _jwtHandler.ExtractIdFromToken();
            string role = _jwtHandler.GetRoleFromToken();

            if (role == "deportista")
            {
                measurementProgressDto.IdAthlete = userID;
            }
            else if (role == "gimnasio" && measurementProgressDto.IdAthlete is not null && measurementProgressDto.IdAthlete > 0)
            {
                int athleteID = Convert.ToInt32(measurementProgressDto.IdAthlete);
                bool hasAthlete = await _unitOfWork.GymRepository.HasAthleteByAthleteID(userID, athleteID);
                if (!hasAthlete)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var validationResults = await _measurementValidationRules.ValidateAsync(measurementProgressDto);

            if (!validationResults.IsValid)
            {
                response.IsSuccess = false;
                response.Errors = validationResults.Errors;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                return response;
            }

            var measurementProgress = _mapper.Map<MeasurementsProgress>(measurementProgressDto);
            measurementProgress.Date = DateOnly.FromDateTime(DateTime.Now);

            response.Data = await _unitOfWork.MeasurementProgressRepository.RecordMeasurementProgress(measurementProgress);

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterAthlete(AthleteRequestDto athleteDto)
        {
            var response = new BaseResponse<bool>();
            string role = _jwtHandler.GetRoleFromToken();

            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var gymID = _jwtHandler.ExtractIdFromToken();
            IDbContextTransaction? transaction = null;

            try
            {
                var gym = await _unitOfWork.GymRepository.GetGymById(gymID);
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
                athlete.AuditCreateUser = gym.GymName;
                athlete.IdGym = gym.GymId;

                var result = await _unitOfWork.AthleteRepository.RegisterAthlete(athlete);

                if (!result)
                {
                    throw new Exception("Error al registrar al atleta");
                }

                var membershipId = athleteDto.MembershipId ?? 0;
                var membershiptDuration = await _unitOfWork.MembershipRepository.GetMembershipById(membershipId);
                var athleteMembership = new AthleteMembership
                {
                    IdAthlete = athlete.AthleteId,
                    IdMembership = membershipId,
                    StartDate = athleteDto?.StartMembershipDate ?? DateOnly.FromDateTime(DateTime.Now),
                    EndDate = athleteDto?.StartMembershipDate?.AddDays(membershiptDuration.DurationInDays) ?? DateOnly.FromDateTime(DateTime.Now.AddDays(membershiptDuration.DurationInDays)),
                };

                var resultAthleteMembership = await _unitOfWork.AthleteMembershipRepository.RegisterAthleteMembership(athleteMembership);
                if (!resultAthleteMembership)
                {
                    throw new Exception("Error al registrar la membresía del atleta");
                }

                //if (athleteDto.CardAccessCode is null)
                //{
                //    throw new Exception("El código de acceso no puede ser nulo");
                //}

                //var existCardAccess = await _unitOfWork.CardAccessRepository.GetActiveAccessByCode(athleteDto.CardAccessCode);

                //if (existCardAccess)
                //{
                //    throw new Exception("El código de acceso ya se encuentra registrado");
                //}

                //var cardAccess = new CardAccess
                //{
                //    IdAthlete = athlete.AthleteId,
                //    CardNumber = athleteDto.CardAccessCode,
                //    ExpirationDate = null,
                //    Status = true,
                //};

                //var resultCardAccess = await _unitOfWork.CardAccessRepository.RegisterCardAccess(cardAccess);
                //if (!resultCardAccess)
                //{
                //    throw new Exception("Error al registrar el código de acceso");
                //}

                transaction.Commit();

                response.IsSuccess = true;
                response.Data = result;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null && dbEx.InnerException.Message.Contains("unique email")) 
                {
                    response.Message = "El correo electrónico ingresado ya está registrado en el sistema. Por favor, verifica los datos o utiliza otro correo electrónico para continuar.";
                }
                else
                {
                    response.Message = "Ocurrió un error inesperado al guardar los cambios. Por favor, inténtelo nuevamente o contacte al soporte técnico.";
                }

                response.IsSuccess = false;
                transaction?.Rollback();
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

        public async Task<BaseResponse<AthleteResponseDto>> RegisterPassword(LoginRequestDto loginRequestDto)
        {
            var response = new BaseResponse<AthleteResponseDto>();
            IDbContextTransaction? transaction = null;

            try
            {
                var athlete = await _unitOfWork.AthleteRepository.LoginAthlete(loginRequestDto.Email) ?? throw new Exception("El atleta no existe");

                if (athlete.Password != null && athlete.Password != "")
                {
                    throw new Exception("El atleta ya cuenta con una contraseña");
                }

                transaction = _context.Database.BeginTransaction();

                var password = BC.HashPassword(loginRequestDto.Password);
                var result = await _unitOfWork.AthleteRepository.RegisterPassword(athlete.AthleteId, password);

                if (!result)
                {
                    throw new Exception("Error al registrar la contraseña");
                }

                transaction.Commit();

                response.IsSuccess = true;
                response.Data = _mapper.Map<AthleteResponseDto>(athlete);
                response.Data.Token = await _jwtHandler.GenerateAthleteToken(athlete);
                response.Data.RefreshToken = await _jwtHandler.GenerateAthleteRefreshToken(athlete);
                response.Message = ReplyMessage.MESSAGE_SAVE;

                bool resultRefreshToken = await RefreshTokenLogic(athlete.AthleteId, response.Data.RefreshToken);

                if (!resultRefreshToken)
                {
                    throw new Exception("Error al refrescar el token");
                }
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

            string role = _jwtHandler.GetRoleFromToken();

            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var gymID = _jwtHandler.ExtractIdFromToken();

            var hasAthlete = await _unitOfWork.GymRepository.HasAthleteByAthleteID(gymID, athleteID);
            if (!hasAthlete)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

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
            string role = _jwtHandler.GetRoleFromToken();

            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var gymID = _jwtHandler.ExtractIdFromToken();

            var hasAthlete = await _unitOfWork.GymRepository.HasAthleteByAthleteID(gymID, membershipToAthleteDto.AthleteId);
            if (!hasAthlete)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

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
                    StartDate = membershipToAthleteDto?.StartMembershipDate ?? DateOnly.FromDateTime(DateTime.Now),
                    EndDate = membershipToAthleteDto?.StartMembershipDate?.AddDays(membershiptDuration.DurationInDays) ?? DateOnly.FromDateTime(DateTime.Now.AddDays(membershiptDuration.DurationInDays)),
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

        public async Task<BaseResponse<int>> VerifyAccessAthlete(VerifyAccessRequestDto verifyAccessDto)
        {
            var response = new BaseResponse<int>();
            var athlete = await _unitOfWork.AthleteRepository.LoginAthlete(verifyAccessDto.Email);

            if (athlete is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                response.Data = 0;
            }
            else
            {
                if (athlete.Password != null && athlete.Password != "")
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                    response.Data = 1;
                }
                else
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    response.Data = 2;
                }
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<MeasurementsByLastMonthResponseDto>>> GetMeasurementsByLastMonth(int athleteID)
        {
            var userID = _jwtHandler.ExtractIdFromToken();
            var response = new BaseResponse<IEnumerable<MeasurementsByLastMonthResponseDto>>();
            string role = _jwtHandler.GetRoleFromToken();

            if (role == "deportista")
            {
                athleteID = userID;
            }
            else if (role == "gimnasio" && athleteID > 0)
            {
                bool hasAthlete = await _unitOfWork.GymRepository.HasAthleteByAthleteID(userID, athleteID);
                if (!hasAthlete)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var measurements = await _unitOfWork.MeasurementProgressRepository.GetMeasurementsByLastMonth(athleteID);

            response.IsSuccess = true;
            response.Data = _mapper.Map<IEnumerable<MeasurementsByLastMonthResponseDto>>(measurements);
            response.Message = ReplyMessage.MESSAGE_QUERY;

            return response;
        }

        public async Task<BaseResponse<AthleteResponseDto>> RefreshAuthToken(string refreshToken)
        {
            var response = new BaseResponse<AthleteResponseDto>();
            string role = _jwtHandler.GetRoleFromRefreshToken(refreshToken);
            string tokenType = _jwtHandler.GetTokenTypeFromRefreshToken(refreshToken);
            bool refreshTokenValid = await _unitOfWork.AthleteTokenRepository.GetRevokeStatus(refreshToken);

            if (role != "deportista" || tokenType != "refresh" || refreshTokenValid == true)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

            var athleteID = _jwtHandler.GetIdFromRefreshToken(refreshToken);
            var athleteFromToken = await _unitOfWork.AthleteRepository.AthleteById(athleteID);

            if (athleteFromToken is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<AthleteResponseDto>(athleteFromToken);
                response.Data.Token = await _jwtHandler.GenerateAthleteToken(athleteFromToken);
                response.Data.RefreshToken = await _jwtHandler.GenerateAthleteRefreshToken(athleteFromToken);
                response.Message = ReplyMessage.MESSAGE_QUERY;

                bool result = await RefreshTokenLogic(athleteFromToken.AthleteId, response.Data.RefreshToken, refreshToken);

                if (!result)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                    response.Data = null;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<AthleteEditResponseDto>> EditAthleteMobile(AthleteEditRequestDto athleteDto)
        {
            var response = new BaseResponse<AthleteEditResponseDto>();

            string role = _jwtHandler.GetRoleFromToken();

            if (role != "deportista")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            IDbContextTransaction? transaction = null;

            try
            {
                var missingFields = new List<string>();

                if (string.IsNullOrWhiteSpace(athleteDto.AthleteName))
                    missingFields.Add("AthleteName");
                if (string.IsNullOrWhiteSpace(athleteDto.AthleteLastName))
                    missingFields.Add("AthleteLastName");
                if (string.IsNullOrWhiteSpace(athleteDto.Email))
                    missingFields.Add("Email");
                if (string.IsNullOrWhiteSpace(athleteDto.PhoneNumber))
                    missingFields.Add("PhoneNumber");
                if (string.IsNullOrWhiteSpace(athleteDto.Genre))
                    missingFields.Add("Genre");

                if (missingFields.Any())
                {
                    throw new Exception("No pueden haber campos nulos o vacios");
                }

                var athleteID = _jwtHandler.ExtractIdFromToken();
                var athleteEdit = await _unitOfWork.AthleteRepository.AthleteById(athleteID) ?? throw new Exception("El atleta no existe");
                transaction = _context.Database.BeginTransaction();

                var athlete = _mapper.Map<Athlete>(athleteDto);
                athlete.AthleteId = athleteID;
                athlete.AuditUpdateDate = DateTime.Now;
                athlete.AuditUpdateUser = Convert.ToString("Athlete" + athleteID);
                athlete.AuditCreateDate = athleteEdit.AuditCreateDate;
                athlete.AuditCreateUser = athleteEdit.AuditCreateUser;
                athlete.Password = athleteEdit.Password;
                athlete.IdGym = athleteEdit.IdGym;
                athlete.Status = athleteEdit.Status;
                bool update = await _unitOfWork.AthleteRepository.EditAthlete(athlete);

                if (!update)
                {
                    throw new Exception("Error al editar al atleta");
                }

                var athleteEdited = await _unitOfWork.AthleteRepository.AthleteById(athleteID);

                response.IsSuccess = true;
                response.Data = _mapper.Map<AthleteEditResponseDto>(athleteEdited);
                response.Message = ReplyMessage.MESSAGE_QUERY;

                transaction.Commit();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<ContactInformationResponseDto>> GetContactInformation()
        {
            var response = new BaseResponse<ContactInformationResponseDto>();
            var contactInformation = await _unitOfWork.ContactInformationRepository.GetContactInformation();

            if (contactInformation is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<ContactInformationResponseDto>(contactInformation);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterAthleteFingerPrint(FingerprintRequest request)
        {
            var response = new BaseResponse<bool>();
            var userID = _jwtHandler.ExtractIdFromToken();
            string role = _jwtHandler.GetRoleFromToken();
            int athleteID = Convert.ToInt32(request.AthleteID);

            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            if (role == "gimnasio" && athleteID > 0)
            {
                bool hasAthlete = await _unitOfWork.GymRepository.HasAthleteByAthleteID(userID, athleteID);
                if (!hasAthlete)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
            }

            IDbContextTransaction? transaction = null;

            try
            {
                var athlete = await _unitOfWork.AthleteRepository.AthleteById(Convert.ToInt32(request.AthleteID)) ?? throw new Exception("El atleta no existe");

                transaction = _context.Database.BeginTransaction();

                var result = await _unitOfWork.AthleteRepository.RegisterAthleteFingerPrint(athlete.AthleteId, request.Fingerprint);

                if (!result)
                {
                    throw new Exception("Error al registrar la huella del atleta");
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

        public async Task<BaseResponse<bool>> AccessAthleteFingerPrint(int athleteID, int? accessType)
        {
            var response = new BaseResponse<bool>();
            var userID = _jwtHandler.ExtractIdFromToken();
            string role = _jwtHandler.GetRoleFromToken();
            int accessTypeValue = accessType ?? 1;

            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            if (role == "gimnasio" && athleteID > 0)
            {
                bool hasAthlete = await _unitOfWork.GymRepository.HasAthleteByAthleteID(userID, athleteID);
                if (!hasAthlete)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
            }

            var access = new AccessLog
            {
                IdAthlete = athleteID,
                IdCard = null,
                AccessDateTime = DateTime.Now,
                IdGym = userID,
                AccessType = accessTypeValue
            };

            var resultAccess = await _unitOfWork.AccessLogRepository.RegisterAccessLog(access);

            if (!resultAccess)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
                response.Data = false;
            }
            else
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
                response.Data = true;
            }

            return response;
        }

        public async Task<bool> DestroyAthleteFromDB(DestroyAthleteRequestDto request)
        {
            var response = false;
            IDbContextTransaction? transaction = null;

            try
            {
                var athlete = await _unitOfWork.AthleteRepository.LoginAthlete(request.Email) ?? throw new Exception("El atleta no existe");

                transaction = _context.Database.BeginTransaction();

                var result = await _unitOfWork.AthleteRepository.DestroyAthleteFromDB(request.Email);

                if (!result)
                {
                    throw new Exception("Error al eliminar al atleta");
                }

                transaction.Commit();

                response = true;
            }
            catch (Exception)
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

        public async Task<BaseResponse<bool>> RegisterAthleteByQR(string gymID, AthleteRequestDto athleteDto)
        {
            var response = new BaseResponse<bool>();
            var salt = GetJwtSecret();
            var secret = GetJwtKey();
            var key = _cryptographyApplication.GenerateUserSpecificKey(secret, salt);
            var iv = _cryptographyApplication.GenerateIV(salt);

            string decryptedBase64 = _cryptographyApplication.DecryptWithAes(gymID, key, iv);

            // decryptedBase64 es una cadena base64 que contiene "email|password" en bytes
            byte[] decryptedBytes = Convert.FromBase64String(decryptedBase64);
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            int decryptedTextInt = Convert.ToInt32(decryptedText);

            try
            {
                var gym = await _unitOfWork.GymRepository.GetGymById(decryptedTextInt);

                var validationResults = await _validationRules.ValidateAsync(athleteDto);

                if (!validationResults.IsValid)
                {
                    response.IsSuccess = false;
                    response.Errors = validationResults.Errors;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    throw new Exception("Error al validar los datos");
                }

                var athlete = _mapper.Map<Athlete>(athleteDto);
                athlete.AuditCreateDate = DateTime.Now;
                athlete.AuditCreateUser = gym.GymName;
                athlete.IdGym = gym.GymId;

                var result = await _unitOfWork.AthleteRepository.RegisterAthlete(athlete);

                if (!result)
                {
                    throw new Exception("Error al registrar al atleta");
                }

                response.IsSuccess = true;
                response.Data = result;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null && dbEx.InnerException.Message.Contains("unique email"))
                {
                    response.Message = "El correo electrónico ingresado ya está registrado en el sistema. Por favor, verifica los datos o utiliza otro correo electrónico para continuar.";
                }
                else
                {
                    response.Message = "Ocurrió un error inesperado al guardar los cambios. Por favor, inténtelo nuevamente o contacte al soporte técnico.";
                }

                response.IsSuccess = false;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }

            return response;
        }
    }
}
