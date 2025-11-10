using MagicVilla_Api.Models;

namespace MagicVilla_Api.Repository.IRepository;

public interface IVillaNumberRepository : IRepository<VillaNumber>
{
    Task<VillaNumber> UpdateVillaNumberAsync(VillaNumber villaNumber);
}