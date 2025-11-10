using System.Linq.Expressions;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;

namespace MagicVilla_Api.Repository.IRepository;

public interface IVillaRepository : IRepository<Villa>
{
    Task<Villa> UpdateVillaAsync(Villa villa);

}