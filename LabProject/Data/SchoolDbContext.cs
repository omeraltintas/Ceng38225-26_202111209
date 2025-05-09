using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LabProject.Models;
namespace LabProject.Data
{
 public class SchoolDbContext : IdentityDbContext
 {
 public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
 : base(options)
 {
 }
 public DbSet<Class> Classes1 { get; set; }
 }
}