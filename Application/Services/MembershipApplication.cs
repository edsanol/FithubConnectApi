using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Application.Validators.Membership;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class MembershipApplication : IMembershipApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MembershipVaidator _validationRules;

        public MembershipApplication(IUnitOfWork unitOfWork, IMapper mapper, MembershipVaidator validationRules)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validationRules = validationRules;
        }

        public async Task<BaseResponse<bool>> CreateMembership(MembershipRequestDto membership)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var validationResults = await _validationRules.ValidateAsync(membership);

                if (!validationResults.IsValid)
                {
                    response.IsSuccess = false;
                    response.Errors = validationResults.Errors;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    return response;
                }

                var membershipCreate = _mapper.Map<Membership>(membership);
                var result = await _unitOfWork.MembershipRepository.CreateMembership(membershipCreate);

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

        public async Task<BaseResponse<bool>> DeleteMembership(int membershipID)
        {
            var response = new BaseResponse<bool>();
            var membership = await MembershipById(membershipID);

            if (membership.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            response.Data = await _unitOfWork.MembershipRepository.DeleteMembership(membershipID);

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

        public async Task<BaseResponse<BaseEntityResponse<MembershipResponseDto>>> ListMemberships(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<MembershipResponseDto>>();
            var memberships = await _unitOfWork.MembershipRepository.ListMemberships(filters);

            if (memberships is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<BaseEntityResponse<MembershipResponseDto>>(memberships);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<MembershipSelectResponseDto>>> ListMembershipsSelect(int gymID)
        {
            var response = new BaseResponse<IEnumerable<MembershipSelectResponseDto>>();
            var memberships = await _unitOfWork.MembershipRepository.ListSelectMemberships(gymID);

            if (memberships is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<IEnumerable<MembershipSelectResponseDto>>(memberships);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<MembershipResponseDto>> MembershipById(int membershipID)
        {
            var response = new BaseResponse<MembershipResponseDto>();
            var membership = await _unitOfWork.MembershipRepository.GetMembershipById(membershipID);

            if (membership is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<MembershipResponseDto>(membership);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> UpdateMembership(int membershipID, MembershipRequestDto membership)
        {
            var response = new BaseResponse<bool>();
            var MembershipEdit = await MembershipById(membershipID);

            if (MembershipEdit.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            var validationResults = await _validationRules.ValidateAsync(membership);

            if (!validationResults.IsValid)
            {
                response.IsSuccess = false;
                response.Errors = validationResults.Errors;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                return response;
            }

            var membershipUpdate = _mapper.Map<Membership>(membership);
            membershipUpdate.MembershipId = membershipID;
            response.Data = await _unitOfWork.MembershipRepository.UpdateMembership(membershipUpdate);

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
    }
}
