using System.Linq.Expressions;
using MagicVilla_Api.Data;
using MagicVilla_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_Api.Repository.IRepository;

public class VillaRepository : Repository<Villa>, IVillaRepository
{
    private readonly ApplicationDbContext _context;
    
    public VillaRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<Villa> UpdateVillaAsync(Villa villa)
    {
        villa.UpdatedDate = DateTime.Now;
        _context.Villas.Update(villa);
        await _context.SaveChangesAsync();

        return villa;
    }

}