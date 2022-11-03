namespace Data.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> source, int page, int pageCount)
    {
        if (page == 0) return source.Take(pageCount);
        int rowsToSkip = 0;
        if (page >= 2) rowsToSkip = (page - 1) * pageCount;
        return source.Skip(rowsToSkip).Take(pageCount);
    }
}