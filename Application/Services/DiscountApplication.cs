using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Application.Validators.Discount;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class DiscountApplication : IDiscountApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly DiscountValidator _validationRules;

        public DiscountApplication(IUnitOfWork unitOfWork, IMapper mapper, DiscountValidator validationRules)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validationRules = validationRules;
        }

        public async Task<BaseResponse<bool>> CreateDiscount(DiscountRequestDto discountDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var validationResults = await _validationRules.ValidateAsync(discountDto);

                if (!validationResults.IsValid)
                {
                    response.IsSuccess = false;
                    response.Errors = validationResults.Errors;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    return response;
                }

                var discountExists = await _unitOfWork.DiscountRepository.DiscountExists(discountDto.IdMembership);

                if (discountExists)
                {
                    response.IsSuccess = false;
                    response.Message = "Membership with discount";
                    return response;
                }

                var discountCreate = _mapper.Map<Discount>(discountDto);
                var result = await _unitOfWork.DiscountRepository.CreateDiscount(discountCreate);

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
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteDiscount(int discountID)
        {
            var response = new BaseResponse<bool>();
            var discount = await DiscountById(discountID);

            if (discount.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            response.Data = await _unitOfWork.DiscountRepository.DeleteDiscount(discountID);

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

        public async Task<BaseResponse<DiscountResponseDto>> DiscountById(int discountID)
        {
            var response = new BaseResponse<DiscountResponseDto>();
            var discount = await _unitOfWork.DiscountRepository.GetDiscountById(discountID);

            if (discount is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<DiscountResponseDto>(discount);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<BaseEntityResponse<DiscountResponseDto>>> ListDiscounts(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<DiscountResponseDto>>();
            var discounts = await _unitOfWork.DiscountRepository.ListDiscounts(filters);

            if (discounts is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<BaseEntityResponse<DiscountResponseDto>>(discounts);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<DiscountSelectResponseDto>>> ListDiscountsSelect(int gymID)
        {
            var response = new BaseResponse<IEnumerable<DiscountSelectResponseDto>>();
            var discounts = await _unitOfWork.DiscountRepository.ListSelectDiscounts(gymID);

            if (discounts is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<IEnumerable<DiscountSelectResponseDto>>(discounts);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> UpdateDiscount(int discountID, DiscountRequestDto discount)
        {
            var response = new BaseResponse<bool>();
            var discountUpdate = await DiscountById(discountID);

            if (discountUpdate.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            var validationResults = await _validationRules.ValidateAsync(discount);

            if (!validationResults.IsValid)
            {
                response.IsSuccess = false;
                response.Errors = validationResults.Errors;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                return response;
            }

            var discountEdit = _mapper.Map<Discount>(discount);
            discountEdit.DiscountId = discountID;
            response.Data = await _unitOfWork.DiscountRepository.UpdateDiscount(discountEdit);

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
