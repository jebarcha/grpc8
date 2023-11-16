using Grpc.Core;
using GrpcMtto.Data;
using GrpcMtto.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcMtto.Services;

public class ProductService : ProductIt.ProductItBase
{
    private readonly GrpcDbContext _dbContext;

    public ProductService(GrpcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<CreateProductResponse> CreateProduct(
        CreateProductRequest request,
        ServerCallContext context
        )
    {
        if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Description))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Send data correctly.")
            );
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description
        };

        await _dbContext.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(
            new CreateProductResponse
            {
                Id = product.Id
            }
        );

    }


    public override async Task<ReadProductResponse> ReadProduct(
        ReadProductRequest request,
        ServerCallContext context)
    {
        if (request.Id <= 0)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Index must be greater than zero.")
            );
        }

        var product = await _dbContext.Products.FirstOrDefaultAsync(t => t.Id == request.Id);

        if (product is not null)
        {
            return await Task.FromResult(
                new ReadProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Status = product.Status
                }
            );
        }

        throw new RpcException(
            new Status(StatusCode.NotFound, $"Product with id {request.Id} does not exist.")
        );
    }


    public override async Task<GetAllResponse> ListProduct(
        GetAllRequest request,
        ServerCallContext context
        )
    {
        var response = new GetAllResponse();
        var products = await _dbContext.Products.ToListAsync();
        foreach (var product in products)
        {
            response.Product.Add(
                new ReadProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Status = product.Status
                }
            );
        }

        return await Task.FromResult(response);
    }

    public override async Task<UpdateProductResponse> UpdateProduct(
        UpdateProductRequest request,
        ServerCallContext context
    )
    {
        if (
            request.Id <= 0 ||
            string.IsNullOrEmpty(request.Name) ||
            string.IsNullOrEmpty(request.Description)
        )
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Must provide correct data")
            );
        }

        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (product is null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, $"Does not exist product with id {request.Id}")
            );
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Status = request.Status;

        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(
            new UpdateProductResponse
            {
                Id = product.Id
            }
        );

    }


    public override async Task<DeleteProductResponse> DeleteProduct(
        DeleteProductRequest request,
        ServerCallContext context
    )
    {

        if (request.Id <= 0)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Product Id is incorrect.")
            );
        }

        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (product is null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, $"Product Id {request.Id} does not exist.")
            );
        }

        _dbContext.Remove(product);

        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(
            new DeleteProductResponse
            {
                Id = product.Id
            }
        );


    }




}