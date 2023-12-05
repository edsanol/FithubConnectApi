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
using Microsoft.Extensions.Configuration;
using Utilities.Static;
using BC = BCrypt.Net.BCrypt;

namespace Application.Services
{
    public class GymApplication : IGymApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly GymValidator _validationRules;
        private readonly IConfiguration _configuration;
        private readonly IEmailServiceApplication _emailService;
        private readonly IJwtHandler _jwtHandler;

        public GymApplication(IUnitOfWork unitOfWork, IMapper mapper, GymValidator validationRules, IConfiguration configuration, IEmailServiceApplication emailService, IJwtHandler jwtHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validationRules = validationRules;
            _configuration = configuration;
            _emailService = emailService;
            _jwtHandler = jwtHandler;
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

        public async Task<BaseResponse<bool>> RegisterGym(GymRequestDto gymDto)
        {
            var response = new BaseResponse<bool>();

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
                var result = await _unitOfWork.GymRepository.RegisterGym(gym);

                if (result)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    response.Message = ReplyMessage.MESSAGE_SAVE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }


            }

            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EditGym(int gymID, GymRequestDto gymDto)
        {
            var response = new BaseResponse<bool>();
            var gymEdit = await _unitOfWork.GymRepository.GetGymById(gymID);

            if (gymEdit is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            var gym = _mapper.Map<Gym>(gymDto);
            gym.GymId = gymID;
            gym.Password = gymEdit!.Password;
            response.Data = await _unitOfWork.GymRepository.EditGym(gym);

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_UPDATE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RemoveGym(int gymID)
        {
            var response = new BaseResponse<bool>();
            var gym = await GymById(gymID);

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

            if (gym is not null)
            {
                if (BC.Verify(loginDto.Password, gym.Password))
                {
                    response.IsSuccess = true;
                    response.Data = _mapper.Map<GymResponseDto>(gym);
                    response.Data.Token = await _jwtHandler.GenerateToken(gym);
                    response.Message = ReplyMessage.MESSAGE_LOGIN;
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

            var gymId = _jwtHandler.ExtractGymIdFromToken(request.Token);

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
    }
}
