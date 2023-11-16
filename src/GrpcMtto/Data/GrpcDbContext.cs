using GrpcMtto.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcMtto.Data;

public class GrpcDbContext : DbContext
{
  public GrpcDbContext(DbContextOptions<GrpcDbContext> options) : base(options)
  {
  }

  public DbSet<Product> Products { get; set; }

}