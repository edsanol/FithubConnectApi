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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        private readonly IConfiguration _configuration;

        public GymApplication(IUnitOfWork unitOfWork, IMapper mapper, GymValidator validationRules, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validationRules = validationRules;
            _configuration = configuration;
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
            var gymEdit = await GymById(gymID);

            if (gymEdit.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            var gym = _mapper.Map<Gym>(gymDto);
            gym.GymId = gymID;
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
                    response.Data.Token = GenerateToken(gym);
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

        public string GenerateToken(Gym gym)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, gym.GymId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, gym.Email.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, Guid.NewGuid().ToString(), ClaimValueTypes.Integer64),
            };
            
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:Expires"]!)),
                notBefore: DateTime.UtcNow,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
