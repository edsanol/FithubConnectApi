using Application.Commons.Bases;
using Application.Dtos.Enum;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Application.Validators.Products;
using Application.Validators.ProductsCategory;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Utilities.Static;

namespace Application.Services
{
    public class InventoryProductsApplication : IInventoryProductsApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtHandler _jwtHandler;
        private readonly ProductsCategoryValidator _productsCategoryValidationRules;
        private readonly ProductsValidator _productsValidationRules;
        private readonly ProductEditValidator _productEditValidationRules;
        private readonly IMapper _mapper;
        private readonly DbFithubContext _context;

        public InventoryProductsApplication(
            IJwtHandler jwtHandler, 
            IUnitOfWork unitOfWork, 
            ProductsCategoryValidator productsCategoryValidationRules, 
            IMapper mapper, 
            ProductsValidator validationRules,
            DbFithubContext context,
            ProductEditValidator productEditValidationRules
            )
        {
            _unitOfWork = unitOfWork;
            _jwtHandler = jwtHandler;
            _productsCategoryValidationRules = productsCategoryValidationRules;
            _mapper = mapper;
            _productsValidationRules = validationRules;
            _context = context;
            _productEditValidationRules = productEditValidationRules;
        }

        public async Task<BaseResponse<bool>> DeleteProduct(int productId)
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
                var product = await _unitOfWork.ProductsRepository.GetProductById(productId, gymID) ?? throw new Exception("Error al obtener el producto");

                if (product.IdGym != gymID)
                {
                    response.IsSuccess = false;
                    response.Message = "No autorizado";
                    return response;
                }

                product.IsActive = false;

                var result = await _unitOfWork.ProductsRepository.EditProduct(product);

                if (!result)
                {
                    throw new Exception("Error al eliminar el producto");
                }

                transaction.Commit();

                response.IsSuccess = true;
                response.Data = result;
                response.Message = ReplyMessage.MESSAGE_DELETE;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EditProduct(int productId, EditProductRequestDto request)
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
                var validationResults = await _productEditValidationRules.ValidateAsync(request);

                if (!validationResults.IsValid)
                {
                    response.IsSuccess = false;
                    response.Errors = validationResults.Errors;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    throw new Exception("Error al validar los datos");
                }

                transaction = _context.Database.BeginTransaction();
                var product = await _unitOfWork.ProductsRepository.GetProductById(productId, gymID) ?? throw new Exception("Error al obtener el producto");

                if (product.IdGym != gymID)
                {
                    response.IsSuccess = false;
                    response.Message = "No autorizado";
                    return response;
                }

                product.Name = request.Name;
                product.Description = request.Description;
                product.IdCategory = request.CategoryId;
                product.BasePrice = request.BasePrice;

                var result = await _unitOfWork.ProductsRepository.EditProduct(product);

                if (!result)
                {
                    throw new Exception("Error al editar el producto");
                }

                var productVariant = await _unitOfWork.ProductsVariantRepository.GetProductVariantByProductId(productId) ?? throw new Exception("Error al obtener la variante del producto");

                productVariant.Price = request.Price;

                var resultVariant = await _unitOfWork.ProductsVariantRepository.EditProductVariant(productVariant);

                if (!resultVariant)
                {
                    throw new Exception("Error al editar la variante del producto");
                }

                transaction.Commit();

                response.IsSuccess = true;
                response.Data = result;
                response.Message = ReplyMessage.MESSAGE_SAVE;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<CategoryProductsResponseDto>>> GetAllCategoriesProducts()
        {
            var response = new BaseResponse<IEnumerable<CategoryProductsResponseDto>>();

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
                var categories = await _unitOfWork.ProductsCategoryRepository.GetAllCategoriesProducts(gymID) ?? throw new Exception("Error al obtener las categorias");
                response.IsSuccess = true;
                response.Data = _mapper.Map<List<CategoryProductsResponseDto>>(categories);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<BaseEntityResponse<ProductsResponseDto>>> GetAllProducts(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<ProductsResponseDto>>();
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
                var products = await _unitOfWork.ProductsRepository.GetAllProducts(filters, gymID) ?? throw new Exception("Error al obtener los productos");
                response.IsSuccess = true;
                response.Data = _mapper.Map<BaseEntityResponse<ProductsResponseDto>>(products);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterCategoryProduct(CategoryProductsRequestDto request)
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
                var validationResults = await _productsCategoryValidationRules.ValidateAsync(request);

                if (!validationResults.IsValid)
                {
                    response.IsSuccess = false;
                    response.Errors = validationResults.Errors;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    throw new Exception("Error al validar los datos");
                }

                var category = _mapper.Map<ProductsCategory>(request);
                category.IdGym = gymID;

                var result = await _unitOfWork.ProductsCategoryRepository.RegisterCategoryProduct(category);

                if (!result)
                {
                    throw new Exception("Error al registrar al atleta");
                }

                response.IsSuccess = true;
                response.Data = result;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterEntryAndExitProduct(EntryAndExitProductRequestDto request)
        {
            var response = new BaseResponse<bool>();
            string role = _jwtHandler.GetRoleFromToken();
            string stockMessage = request.Type == MovementTypeEnum.Entry ? "entrada" : "salida";

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
                var product = await _unitOfWork.ProductsRepository.GetProductById(request.ProductId, gymID) ?? throw new Exception("Error al obtener el producto");

                if (product.IdGym != gymID)
                {
                    response.IsSuccess = false;
                    response.Message = "No autorizado";
                    return response;
                }

                var productVariant = await _unitOfWork.ProductsVariantRepository.GetProductVariantByProductId(request.ProductId) ?? throw new Exception("Error al obtener la variante del producto");

                if (request.Type == MovementTypeEnum.Entry)
                {
                    productVariant.StockQuantity += request.Quantity;
                }
                else
                {
                    if (productVariant.StockQuantity < request.Quantity)
                    {
                        response.IsSuccess = false;
                        response.Message = "No hay suficiente stock";
                        return response;
                    }

                    productVariant.StockQuantity -= request.Quantity;
                }

                var result = await _unitOfWork.ProductsVariantRepository.EditProductVariant(productVariant);

                if (!result)
                {
                    throw new Exception("Error al registrar la entrada o salida del producto");
                }

                var stockMovement = new StockMovements
                {
                    IdVariant = productVariant.VariantId,
                    MovementType = request.Type.ToString(),
                    Quantity = request.Quantity,
                    MovementDate = DateTime.Now,
                    Notes = stockMessage
                };

                var resultStockMovement = await _unitOfWork.StockMovementsRepository.RegisterEntryAndExitProduct(stockMovement);

                if (!resultStockMovement)
                {
                    throw new Exception("Error al registrar la entrada o salida del producto");
                }

                transaction.Commit();

                response.IsSuccess = true;
                response.Data = result;
                response.Message = ReplyMessage.MESSAGE_SAVE;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterProduct(ProductsRequestDto request)
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
                var validationResults = await _productsValidationRules.ValidateAsync(request);

                if (!validationResults.IsValid)
                {
                    response.IsSuccess = false;
                    response.Errors = validationResults.Errors;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    throw new Exception("Error al validar los datos");
                }

                transaction = _context.Database.BeginTransaction();
                var product = _mapper.Map<Products>(request);
                product.IdGym = gymID;

                var result = await _unitOfWork.ProductsRepository.RegisterProduct(product);

                if (!result)
                {
                    throw new Exception("Error al registrar el producto");
                }

                var productVariant = new ProductsVariant
                {
                    IdProduct = product.ProductId,
                    SKU = request.SKU,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity
                };

                var resultVariant = await _unitOfWork.ProductsVariantRepository.RegisterProductVariant(productVariant);

                if (!resultVariant)
                {
                    throw new Exception("Error al registrar la variante del producto");
                }

                var stockMovement = new StockMovements
                {
                    IdVariant = productVariant.VariantId,
                    MovementType = MovementTypeEnum.Entry.ToString(),
                    Quantity = request.StockQuantity,
                    MovementDate = DateTime.Now,
                    Notes = "entrada"
                };

                var resultStockMovement = await _unitOfWork.StockMovementsRepository.RegisterEntryAndExitProduct(stockMovement);

                if (!resultStockMovement)
                {
                    throw new Exception("Error al registrar la entrada del producto");
                }

                transaction.Commit();

                response.IsSuccess = true;
                response.Data = result;
                response.Message = ReplyMessage.MESSAGE_SAVE;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                transaction?.Dispose();
            }

            return response;
        }
    }
}
