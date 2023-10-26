namespace IntegrationEventLog;

public class ResilientTransaction
{
    private readonly DbContext _context;

    private ResilientTransaction(DbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public static ResilientTransaction New(DbContext context) => new(context);

    public async Task ExecuteAsync(Func<Task> action)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                await action();
                transaction.Complete();
            }
        });
    }
}