using MagicVilla_Api.Data;
using MagicVilla_Api.Models;

namespace MagicVilla_Api.Repository.IRepository;

public class VillaNumberRepository : Repository<VillaNumber>,IVillaNumberRepository
{
    private readonly ApplicationDbContext _context;

    public VillaNumberRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<VillaNumber> UpdateVillaNumberAsync(VillaNumber villaNumber)
    {
        villaNumber.UpdatedDate = DateTime.Now;
        _context.VillaNumbers.Update(villaNumber);
        await _context.SaveChangesAsync();

        return villaNumber;
    }
}