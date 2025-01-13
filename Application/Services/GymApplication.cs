using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Application.Validators.Gym;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Text;
using Utilities.Static;
using BC = BCrypt.Net.BCrypt;

namespace Application.Services
{
    public class GymApplication : IGymApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly GymValidator _validationRules;
        private readonly IEmailServiceApplication _emailService;
        private readonly IJwtHandler _jwtHandler;
        private readonly ICryptographyApplication _cryptographyApplication;
        private readonly IConfiguration _configuration;

        public GymApplication(
            IUnitOfWork unitOfWork, 
            IMapper mapper, GymValidator validationRules, 
            IEmailServiceApplication emailService, 
            IJwtHandler jwtHandler, 
            ICryptographyApplication cryptographyApplication,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validationRules = validationRules;
            _emailService = emailService;
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
            return Environment.GetEnvironmentVariable("ID_SECRET")
                ?? _configuration["Jwt:Key"]!;
        }

        private async Task<bool> RefreshTokenLogic(int gymID, string refreshToken, string actualRefreshToken = "")
        {
            if (!string.IsNullOrEmpty(actualRefreshToken) && actualRefreshToken.Length > 0)
            {
                var revokeTokenStatus = await _unitOfWork.GymTokenRepository.RevokeGymToken(actualRefreshToken);

                if (!revokeTokenStatus)
                {
                    return false;
                }
            }

            var newRefreshToken = new GymToken
            {
                TokenID = 0,
                IdGym = gymID,
                Token = refreshToken,
                Expires = DateOnly.FromDateTime(DateTime.Now.AddMinutes(21600)),
                Revoked = false,
            };

            bool result = await _unitOfWork.GymTokenRepository.RegisterGymToken(newRefreshToken);

            if (!result)
            {
                return false;
            }

            return true;
        }

        public async Task<BaseResponse<BaseEntityResponse<GymResponseDto>>> ListGyms(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<GymResponseDto>>();
            var gyms = await _unitOfWork.GymRepository.ListGyms(filters);

            if (gyms is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<BaseEntityResponse<GymResponseDto>>(gyms);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<GymSelectResponseDto>>> ListGymsSelect()
        {
            var response = new BaseResponse<IEnumerable<GymSelectResponseDto>>();
            var gyms = await _unitOfWork.GymRepository.ListSelectGyms();

            if (gyms is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<IEnumerable<GymSelectResponseDto>>(gyms);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<GymResponseDto>> GymById(int gymID)
        {
            var response = new BaseResponse<GymResponseDto>();
            string role = _jwtHandler.GetRoleFromToken();
            int gymIdQuery;

            if (role == "gimnasio")
            {
                var userID = _jwtHandler.ExtractIdFromToken();
                gymIdQuery = userID;
            }
            else if (role == "deportista" && gymID > 0)
            {
                gymIdQuery = gymID;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var gym = await _unitOfWork.GymRepository.GetGymById(gymIdQuery);

            if (gym is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<GymResponseDto>(gym);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterGym(GymRequestDto gymDto)
        {
            var response = new BaseResponse<bool>();
            IDbContextTransaction? transaction = null;

            try
            {
                var validationResults = await _validationRules.ValidateAsync(gymDto);

                if (!validationResults.IsValid)
                {
                    response.IsSuccess = false;
                    response.Errors = validationResults.Errors;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    return response;
                }

                var gym = _mapper.Map<Gym>(gymDto);
                gym.Password = BC.HashPassword(gymDto.Password);

                var gymResult = await _unitOfWork.GymRepository.RegisterGym(gym);

                if (!gymResult)
                {
                    transaction?.Rollback();
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                    return response;
                }

                // Registrar los tipos de acceso asociados
                if (gymDto.AccessTypeIds != null && gymDto.AccessTypeIds.Any())
                {
                    foreach (var accessTypeId in gymDto.AccessTypeIds)
                    {
                        var gymAccessType = new GymAccessType
                        {
                            IdGym = gym.GymId,
                            IdAccessType = accessTypeId
                        };

                        var gymAccessTypeResult = await _unitOfWork.GymAccessTypeRepository.CreateGymAccessType(gymAccessType);

                        if (!gymAccessTypeResult)
                        {
                            transaction?.Rollback();
                            response.IsSuccess = false;
                            response.Message = ReplyMessage.MESSAGE_FAILED;
                            return response;
                        }
                    }
                }

                transaction?.Commit();

                response.IsSuccess = true;
                response.Data = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }

            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EditGym(GymRequestDto gymDto)
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
            var gymEdit = await _unitOfWork.GymRepository.GetGymById(gymID);

            if (gymEdit is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            var gym = _mapper.Map<Gym>(gymDto);
            gym.GymId = gymID;
            gym.Password = gymEdit!.Password;

            //response.Data = await _unitOfWork.GymRepository.EditGym(gym);

            //if (response.Data)
            //{
            //    response.IsSuccess = true;
            //    response.Message = ReplyMessage.MESSAGE_UPDATE;
            //}
            //else
            //{
            //    response.IsSuccess = false;
            //    response.Message = ReplyMessage.MESSAGE_FAILED;
            //}

            //return response;

            IDbContextTransaction? transaction = null;

            try
            {
                var gymResult = await _unitOfWork.GymRepository.EditGym(gym);

                if (!gymResult)
                {
                    transaction?.Rollback();
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                    return response;
                }

                // Eliminar los tipos de acceso asociados
                var gymAccessTypes = await _unitOfWork.GymAccessTypeRepository.GetGymAccessTypesByGymID(gym.GymId);

                if (gymAccessTypes != null && gymAccessTypes.Any())
                {
                    foreach (var gymAccessType in gymAccessTypes)
                    {
                        var gymAccessTypeResult = await _unitOfWork.GymAccessTypeRepository.DeleteGymAccessType(gymAccessType.GymAccessTypeId);

                        if (!gymAccessTypeResult)
                        {
                            transaction?.Rollback();
                            response.IsSuccess = false;
                            response.Message = ReplyMessage.MESSAGE_FAILED;
                            return response;
                        }
                    }
                }

                // Registrar los tipos de acceso asociados
                if (gymDto.AccessTypeIds != null && gymDto.AccessTypeIds.Any())
                {
                    foreach (var accessTypeId in gymDto.AccessTypeIds)
                    {
                        var gymAccessType = new GymAccessType
                        {
                            IdGym = gym.GymId,
                            IdAccessType = accessTypeId
                        };

                        var gymAccessTypeResult = await _unitOfWork.GymAccessTypeRepository.CreateGymAccessType(gymAccessType);

                        if (!gymAccessTypeResult)
                        {
                            transaction?.Rollback();
                            response.IsSuccess = false;
                            response.Message = ReplyMessage.MESSAGE_FAILED;
                            return response;
                        }
                    }
                }

                transaction?.Commit();

                response.IsSuccess = true;
                response.Data = true;
                response.Message = ReplyMessage.MESSAGE_UPDATE;
            }

            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RemoveGym()
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
            var gym = await GymByIdRemove(gymID);

            if (gym.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            response.Data = await _unitOfWork.GymRepository.DeleteGym(gymID);

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

        public async Task<BaseResponse<GymResponseDto>> LoginGym(LoginRequestDto loginDto)
        {
            var response = new BaseResponse<GymResponseDto>();
            var gym = await _unitOfWork.GymRepository.LoginGym(loginDto.Email);
            var salt = GetJwtSecret();

            if (gym is not null)
            {
                if (BC.Verify(loginDto.Password, gym.Password))
                {
                    response.IsSuccess = true;
                    response.Data = _mapper.Map<GymResponseDto>(gym);

                    var secret = GetJwtKey();
                    var key = _cryptographyApplication.GenerateUserSpecificKey(secret, salt);
                    var iv = _cryptographyApplication.GenerateIV(salt);
                    var encryptedIdBytes = Encoding.UTF8.GetBytes(gym.GymId.ToString());
                    var encryptedIdBase64 = Convert.ToBase64String(encryptedIdBytes);

                    var encryptedId = _cryptographyApplication.EncryptWithAes(encryptedIdBase64, key, iv);
                    var safeEncryptedId = Uri.EscapeDataString(encryptedId);

                    response.Data.Token = await _jwtHandler.GenerateToken(gym);
                    response.Data.RefreshToken = await _jwtHandler.GenerateRefreshToken(gym);
                    response.Data.EncryptedId = safeEncryptedId;
                    response.Message = ReplyMessage.MESSAGE_LOGIN;

                    bool result = await RefreshTokenLogic(gym.GymId, response.Data.RefreshToken);

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
                    response.Message = ReplyMessage.MESSAGE_LOGIN_FAILED;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_LOGIN_FAILED;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RecoverPassword(RecoverPasswordRequestDto request)
        {
            var response = new BaseResponse<bool>();
            var gym = await _unitOfWork.GymRepository.LoginGym(request.Email);

            if (gym == null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

            var token = await _jwtHandler.GeneratePasswordResetToken(gym.GymId);
            var resetLink = $"https://tuapp.com/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(request.Email)}";
            var emailBody = $"<p>Por favor haz clic en el siguiente enlace para restablecer tu contraseña:</p><p><a href='{resetLink}'>{resetLink}</a></p>";

            await _emailService.SendEmailAsync(request.Email, "Recuperación de Contraseña", emailBody);

            response.IsSuccess = true;
            response.Message = "Password reset token generated.";
            response.Data = true;

            return response;
        }

        public async Task<BaseResponse<bool>> ResetPassword(PasswordResetRequestDto request)
        {
            var response = new BaseResponse<bool>();

            if (request.NewPassword != request.ConfirmPassword)
            {
                response.IsSuccess = false;
                response.Message = "Passwords do not match";
                return response;
            }

            if (request.Token == null || request.Token == string.Empty)
            {
                response.IsSuccess = false;
                response.Message = "Invalid or expired token";
                return response;
            }

            bool isValidToken = _jwtHandler.ValidateToken(request.Token);

            if (!isValidToken)
            {
                response.IsSuccess = false;
                response.Message = "Invalid or expired token";
                return response;
            }

            var gymId = _jwtHandler.ExtractIdFromToken();

            if (gymId == 0)
            {
                response.IsSuccess = false;
                response.Message = "Invalid or expired token";
                return response;
            }

            var password = BC.HashPassword(request.NewPassword);

            if (password == null || password == string.Empty)
            {
                response.IsSuccess = false;
                response.Message = "Invalid password";
                return response;
            }

            var result = await _unitOfWork.GymRepository.ResetPasswordAsync(gymId, password);

            if (!result)
            {
                response.IsSuccess = false;
                response.Message = "Failed to reset password";
                return response;
            }

            response.IsSuccess = result;
            response.Data = result;
            response.Message = result ? "Password reset successfully" : "Failed to reset password";
            return response;
        }

        public async Task<BaseResponse<bool>> ChangePassword(ChangePasswordRequestDto request)
        {
            var response = new BaseResponse<bool>();
            var gym = await _unitOfWork.GymRepository.LoginGym(request.Email!);

            if (gym == null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                response.IsSuccess = false;
                response.Message = "Passwords do not match";
                return response;
            }

            if (!BC.Verify(request.OldPassword, gym.Password))
            {
                response.IsSuccess = false;
                response.Message = "Invalid old password";
                return response;
            }

            var password = BC.HashPassword(request.NewPassword);

            response.IsSuccess = true;
            response.Data = await _unitOfWork.GymRepository.ChangePasswordAsync(gym.GymId, password);
            response.Message = response.Data ? "Password changed successfully" : "Failed to change password";
            return response;
        }

        public async Task<BaseResponse<GymResponseDto>> RefreshAuthToken(string refreshToken)
        {
            var response = new BaseResponse<GymResponseDto>();
            string role = _jwtHandler.GetRoleFromRefreshToken(refreshToken);
            string tokenType = _jwtHandler.GetTokenTypeFromRefreshToken(refreshToken);
            bool refreshTokenValid = await _unitOfWork.GymTokenRepository.GetRevokeStatus(refreshToken);

            if (role != "gimnasio" || tokenType != "refresh" || refreshTokenValid == true)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

            var gymId = _jwtHandler.GetIdFromRefreshToken(refreshToken);
            var gym = await _unitOfWork.GymRepository.GetGymById(gymId);

            if (gym is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<GymResponseDto>(gym);
                response.Data.Token = await _jwtHandler.GenerateToken(gym);
                response.Data.RefreshToken = await _jwtHandler.GenerateRefreshToken(gym);
                response.Message = ReplyMessage.MESSAGE_LOGIN;

                bool result = await RefreshTokenLogic(gym.GymId, response.Data.RefreshToken, refreshToken);

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
                response.Message = ReplyMessage.MESSAGE_LOGIN_FAILED;
            }

            return response;
        }

        private async Task<BaseResponse<GymResponseDto>> GymByIdRemove(int gymID)
        {
            var response = new BaseResponse<GymResponseDto>();
            var gym = await _unitOfWork.GymRepository.GetGymById(gymID);

            if (gym is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<GymResponseDto>(gym);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<AccessTypeResponseDto>>> ListAccessTypes()
        {
            var response = new BaseResponse<IEnumerable<AccessTypeResponseDto>>();
            var accessTypes = await _unitOfWork.AccessTypeRepository.ListAccessTypes();

            if (accessTypes is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<IEnumerable<AccessTypeResponseDto>>(accessTypes);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }
    }
}
