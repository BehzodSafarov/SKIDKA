using ArzonOL.Data;
using ArzonOL.Repositories.Interfaces;

namespace ArzonOL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public ICategoryApproachRepository CategoryApproachRepository { get; }
    public IVoterRepository VoterRepository  { get; }
    public IProductRepository ProductRepository  { get; }
    public IUserRepository UserRepository  { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IBoughtProductRepository BoughtProductRepository {get;}
    public ICartEntityRepository CartRepository {get;}
    public ICartProductRepository CartProductRepository {get;}

    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        CategoryApproachRepository = new CategoryApproachRepository(_context);
        VoterRepository = new VoterRepository(_context);
        ProductRepository = new ProductRepository(_context);
        UserRepository = new UserRepository(_context);
        CategoryRepository = new CategoryRepository(_context);
        BoughtProductRepository = new BoughtProductRepository(_context);
        CartRepository = new CartEntityRepository(_context);
        CartProductRepository = new CartProductRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public Task<int> SaveAsync()
    {
        return _context.SaveChangesAsync();
    }
}