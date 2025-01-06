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
        private readonly IPushNotificationService _pushNotificationService;

        public NotificationApplication(
            IUnitOfWork unitOfWork, 
            IJwtHandler jwtHandler, 
            DbFithubContext _context, 
            IPushNotificationService pushNotificationService
        )
        {
            _unitOfWork = unitOfWork;
            _jwtHandler = jwtHandler;
            this._context = _context;
            _pushNotificationService = pushNotificationService;
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

                if (userChannelRequestDto.AllUsersSelected == false && userChannelRequestDto.AllUsersSelectedByMembersip == false)
                {
                    var channelUsers = userChannelRequestDto.UserIds.Select(userId => new ChannelUsers
                    {
                        IdChannel = userChannelRequestDto.ChannelId,
                        IdAthlete = userId
                    }).ToList();

                    var addUsersToChannelResult = await _unitOfWork.ChannelUsersRepository
                        .AddUsersToChannel(channelUsers, userChannelRequestDto.ChannelId);

                }
                else if (userChannelRequestDto.AllUsersSelected == true && userChannelRequestDto.AllUsersSelectedByMembersip == false)
                {
                    var allAthletes = await _unitOfWork.AthleteRepository.GetAllAthletesByGymID(gymID);

                    var athletes = allAthletes.Where(a => !userChannelRequestDto.DeselectedUserIds.Contains(a.AthleteId)).ToList();

                    var channelUsers = athletes.Select(athlete => new ChannelUsers
                    {
                        IdChannel = userChannelRequestDto.ChannelId,
                        IdAthlete = athlete.AthleteId
                    }).ToList();

                    var addUsersToChannelResult = await _unitOfWork.ChannelUsersRepository
                        .AddUsersToChannel(channelUsers, userChannelRequestDto.ChannelId);

                }
                else if (userChannelRequestDto.AllUsersSelected == false && userChannelRequestDto.AllUsersSelectedByMembersip == true)
                {
                    var allAthletesByMembership = await _unitOfWork.AthleteRepository
                        .GetAllAthletesByMembershipID(userChannelRequestDto.MembershipIds);

                    var athletes = allAthletesByMembership.Where(a => 
                        !userChannelRequestDto.DeselectedUserIds.Contains(a.AthleteId)).ToList();

                    var channelUsers = athletes.Select(athlete => new ChannelUsers
                    {
                        IdChannel = userChannelRequestDto.ChannelId,
                        IdAthlete = athlete.AthleteId
                    }).ToList();

                    var addUsersToChannelResult = await _unitOfWork.ChannelUsersRepository
                        .AddUsersToChannel(channelUsers, userChannelRequestDto.ChannelId);
                }

                response.IsSuccess = true;
                response.Data = true;
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

                if (channelRequestDto.AllUsersSelected == false && channelRequestDto.AllUsersSelectedByMembersip == false)
                {
                    var channelUsers = channelRequestDto.UserIds.Select(userId => new ChannelUsers
                    {
                        IdChannel = channel.ChannelId,
                        IdAthlete = userId
                    }).ToList();

                    var addUsersToChannelResult = await _unitOfWork.ChannelUsersRepository
                        .AddUsersToChannel(channelUsers, channel.ChannelId);

                } else if (channelRequestDto.AllUsersSelected == true && channelRequestDto.AllUsersSelectedByMembersip == false)
                {
                    var allAthletes = await _unitOfWork.AthleteRepository.GetAllAthletesByGymID(gymID);

                    var athletes = allAthletes.Where(a => !channelRequestDto.DeselectedUserIds.Contains(a.AthleteId)).ToList();

                    var channelUsers = athletes.Select(athlete => new ChannelUsers
                    {
                        IdChannel = channel.ChannelId,
                        IdAthlete = athlete.AthleteId
                    }).ToList();

                    var addUsersToChannelResult = await _unitOfWork.ChannelUsersRepository
                        .AddUsersToChannel(channelUsers, channel.ChannelId);

                } else if (channelRequestDto.AllUsersSelected == false && channelRequestDto.AllUsersSelectedByMembersip == true)
                {
                    var allAthletesByMembership = await _unitOfWork.AthleteRepository
                        .GetAllAthletesByMembershipID(channelRequestDto.MembershipIds);

                    var athletes = allAthletesByMembership.Where(a => 
                        !channelRequestDto.DeselectedUserIds.Contains(a.AthleteId)).ToList();

                    var channelUsers = athletes.Select(athlete => new ChannelUsers
                    {
                        IdChannel = channel.ChannelId,
                        IdAthlete = athlete.AthleteId
                    }).ToList();

                    var addUsersToChannelResult = await _unitOfWork.ChannelUsersRepository
                        .AddUsersToChannel(channelUsers, channel.ChannelId);
                }

                response.IsSuccess = true;
                response.Data = true;
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
                    }).ToList(),
                    LastMessage = channel.Notifications.OrderByDescending(n => n.SendAt).FirstOrDefault()?.Message ?? string.Empty
                }).ToList().OrderByDescending(c => c.LastMessage).ToList();

                response.IsSuccess = true;
                response.Message = "Canales obtenidos correctamente";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<List<long>>> GetChannelsByAthlete()
        {
            var response = new BaseResponse<List<long>>();

            string role = _jwtHandler.GetRoleFromToken();

            if (role != "deportista")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var athleteID = _jwtHandler.ExtractIdFromToken();

            try
            {
                var channelsIds = await _unitOfWork.ChannelRepository.GetChannelsByAthleteId(athleteID)
                    ?? throw new Exception("Error al obtener los canales");

                response.IsSuccess = true;
                response.Data = channelsIds;
                response.Message = "Canales obtenidos correctamente";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<List<NotificationResponseDto>>> GetNotificationsByAthlete()
        {
            var response = new BaseResponse<List<NotificationResponseDto>>();

            string role = _jwtHandler.GetRoleFromToken();

            if (role != "deportista")
            {
                response.IsSuccess = false;
                response.Message = "No autorizado";
                return response;
            }

            var athleteID = _jwtHandler.ExtractIdFromToken();

            try
            {
                var notifications = await _unitOfWork.NotificationRepository.GetNotificationsByAthlete(athleteID);

                response.Data = notifications.Select(n => new NotificationResponseDto
                {
                    NotificationId = n.NotificationId,
                    ChannelId = n.IdChannel,
                    Message = n.Message,
                    SendAt = n.SendAt,
                    Type = n.Type,
                    Title = n.Title
                }).ToList();

                response.IsSuccess = true;
                response.Message = "Historial obtenido correctamente";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<List<NotificationResponseDto>>> GetNotificationsByChannel(long channelId)
        {
            var response = new BaseResponse<List<NotificationResponseDto>>();

            try
            {
                var notifications = await _unitOfWork.NotificationRepository.GetNotificationsByChannel(channelId);

                response.Data = notifications.Select(n => new NotificationResponseDto
                {
                    NotificationId = n.NotificationId,
                    ChannelId = n.IdChannel,
                    Message = n.Message,
                    SendAt = n.SendAt
                }).ToList();

                response.IsSuccess = true;
                response.Message = "Historial obtenido correctamente";
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
                    SendAt = DateTime.Now,
                    Type = notificationRequestDto.Type,
                    Title = notificationRequestDto.Title
                };

                var saveResult = await _unitOfWork.NotificationRepository.SaveNotification(notification);

                if (!saveResult)
                {
                    throw new Exception("Error al guardar la notificación");
                }

                var athleteIds = await _unitOfWork.ChannelUsersRepository
                    .GetAllAthleteIdsByChannel(notificationRequestDto.ChannelId);

                var deviceTokens = await _unitOfWork.UserDeviceTokenRepository
                    .GetDeviceTokensByAthleteIds(athleteIds);

                await _pushNotificationService.SendPushNotificationAsync(
                    deviceTokens,
                    notificationRequestDto.Title,
                    notificationRequestDto.Message
                );

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
