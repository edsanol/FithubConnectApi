using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Services
{
    public class NotificationApplication : INotificationApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtHandler _jwtHandler;
        private readonly DbFithubContext _context;

        public NotificationApplication(IUnitOfWork unitOfWork, IJwtHandler jwtHandler, DbFithubContext _context)
        {
            _unitOfWork = unitOfWork;
            _jwtHandler = jwtHandler;
            this._context = _context;
        }

        public async Task<BaseResponse<bool>> AddUserToChannel(UserChannelRequestDto userChannelRequestDto)
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

            try
            {
                transaction = _context.Database.BeginTransaction();

                var channelUsers = userChannelRequestDto.UserIds.Select(userId => new ChannelUsers
                {
                    IdChannel = userChannelRequestDto.ChannelId,
                    IdAthlete = userId
                }).ToList();

                var addUsersToChannelResult = await _unitOfWork.ChannelUsersRepository.AddUsersToChannel(channelUsers);

                if (!addUsersToChannelResult)
                {
                    throw new Exception("Error al registrar al atleta");
                }

                response.IsSuccess = true;
                response.Data = addUsersToChannelResult;
                response.Message = "Atleta registrado correctamente";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<bool>> CreateChannel(ChannelRequestDto channelRequestDto)
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

            try
            {
                transaction = _context.Database.BeginTransaction();

                var channel = new Channels
                {
                    ChannelName = channelRequestDto.Name,
                    IdGym = gymID,
                    CreatedAt = DateTime.Now
                };

                var createChannelResult = await _unitOfWork.ChannelRepository.CreateChannel(channel);

                if (!createChannelResult)
                {
                    throw new Exception("Error al registrar al atleta");
                }

                var channelUsers = channelRequestDto.UserIds.Select(userId => new ChannelUsers
                {
                    IdChannel = channel.ChannelId,
                    IdAthlete = userId
                }).ToList();

                var addUsersToChannelResult = await _unitOfWork.ChannelUsersRepository.AddUsersToChannel(channelUsers);

                if (!addUsersToChannelResult)
                {
                    throw new Exception("Error al registrar al atleta");
                }

                response.IsSuccess = true;
                response.Data = addUsersToChannelResult;
                response.Message = "Canal creado correctamente";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<List<ChannelResponseDto>>> GetChannels()
        {
            var response = new BaseResponse<List<ChannelResponseDto>>();

            string role = _jwtHandler.GetRoleFromToken();

            if (role != "gimnasio")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var gymID = _jwtHandler.ExtractIdFromToken();

            try
            {
                var channels = await _unitOfWork.ChannelRepository.GetChannelsByGymId(gymID) 
                    ?? throw new Exception("Error al obtener los canales");

                response.IsSuccess = true;
                response.Data = channels.Select(channel => new ChannelResponseDto
                {
                    ChannelId = (int)channel.ChannelId,
                    ChannelName = channel.ChannelName,
                    ChannelAthletes = channel.ChannelUsers.Select(channelUser => new ChannelAthletesDto
                    {
                        AthleteId = channelUser.IdAthlete,
                        AthleteName = channelUser.IdAthleteNavigation.AthleteName + " " + channelUser.IdAthleteNavigation.AthleteLastName 
                            ?? string.Empty
                    }).ToList()
                }).ToList();
                response.Message = "Canales obtenidos correctamente";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }


        public async Task<BaseResponse<bool>> RemoveUserFromChannel(UserChannelRequestDto userChannelRequestDto)
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

            try
            {
                transaction = _context.Database.BeginTransaction();

                var channelUsers = userChannelRequestDto.UserIds.Select(userId => new ChannelUsers
                {
                    IdChannel = userChannelRequestDto.ChannelId,
                    IdAthlete = userId
                }).ToList();

                var removeUsersFromChannelResult = await _unitOfWork.ChannelUsersRepository.RemoveUsersFromChannel(channelUsers);

                if (!removeUsersFromChannelResult)
                {
                    throw new Exception("Error al registrar al atleta");
                }

                response.IsSuccess = true;
                response.Data = removeUsersFromChannelResult;
                response.Message = "Atleta eliminado correctamente";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<bool>> SendNotification(NotificationRequestDto notificationRequestDto)
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

            try
            {
                // Verifica que el canal pertenece al gimnasio
                var channel = await _unitOfWork.ChannelRepository.GetChannelById(notificationRequestDto.ChannelId);
                if (channel == null || channel.IdGym != gymID)
                {
                    throw new Exception("Canal no encontrado o no pertenece al gimnasio.");
                }

                var notification = new Notifications
                {
                    IdChannel = notificationRequestDto.ChannelId,
                    Message = notificationRequestDto.Message,
                    CreatedAt = DateTime.Now
                };

                var saveResult = await _unitOfWork.NotificationRepository.SaveNotification(notification);

                if (!saveResult)
                {
                    throw new Exception("Error al guardar la notificación");
                }

                response.IsSuccess = true;
                response.Data = true;
                response.Message = "Notificación creada correctamente";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
