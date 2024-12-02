using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Services
{
    internal class OrdersPaymentsApplication : IOrdersPaymentsApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbFithubContext _context;
        private readonly IJwtHandler _jwtHandler;

        public OrdersPaymentsApplication(IUnitOfWork unitOfWork, DbFithubContext context, IJwtHandler jwtHandler)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _jwtHandler = jwtHandler;
        }

        public async Task<BaseResponse<bool>> RegisterOrder(OrderRequestDto request)
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
                transaction = _context.Database.BeginTransaction();

                var order = new Orders
                {
                    IdAthlete = request.AthleteId,
                    IdGym = gymID,
                    OrderDate = DateTime.Now,
                    TotalAmount = request.TotalAmount,
                    TotalPaid = 0,
                    Status = "Pending",
                    ShippingAddress = request.ShippingAddress,
                    Notes = request.Notes
                };

                var responseOrder = await _unitOfWork.OrdersPaymentsRepository.RegisterOrder(order);

                if (!responseOrder)
                {
                    throw new Exception("Error al registrar la orden");
                }

                //foreach (var orderDetail in request.OrderDetails)
                //{
                //    var orderDetailEntity = new OrderDetails
                //    {
                //        IdOrder = order.OrderId,
                //        IdVariant = orderDetail.VariantId,
                //        Quantity = orderDetail.Quantity,
                //        UnitPrice = 
                //    };

                //    var responseOrderDetail = await _unitOfWork.OrdersPaymentsRepository.RegisterOrderDetail(orderDetailEntity);

                //    if (!responseOrderDetail)
                //    {
                //        throw new Exception("Error al registrar el detalle de la orden");
                //    }
                //}
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                    transaction?.Dispose();
                }
            }

            return response;
        }
    }
}
