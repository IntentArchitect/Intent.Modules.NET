using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Domain.Guardrails.Contracts.RepositoryContract", Version = "1.0")]

namespace eShop.Ordering.Infrastructure.Repositories;

public class BuyerRepository
    : IBuyerRepository
{
    private readonly OrderingContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public BuyerRepository(OrderingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Buyer Add(Buyer buyer)
    {
        if (buyer.IsTransient())
        {
            return _context.Buyers
                .Add(buyer)
                .Entity;
        }

        return buyer;
    }

    public Buyer Update(Buyer buyer)
    {
        return _context.Buyers
                .Update(buyer)
                .Entity;
    }

    public async Task<Buyer> FindAsync(string identity)
    {
        var buyer = await _context.Buyers
            .Include(b => b.PaymentMethods)
            .Where(b => b.IdentityGuid == identity)
            .SingleOrDefaultAsync();

        return buyer;
    }

    public async Task<Buyer> FindByIdAsync(int id)
    {
        var buyer = await _context.Buyers
            .Include(b => b.PaymentMethods)
            .Where(b => b.Id == id)
            .SingleOrDefaultAsync();

        return buyer;
    }
}
