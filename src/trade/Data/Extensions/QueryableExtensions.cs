namespace Data.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TProjection> Paginate<TProjection>(this IQueryable<TProjection> source, int page, int pageCount)
    {
        if (page == 0) return source;
        int rowsToSkip = 0;
        if (page >= 2) rowsToSkip = (page - 1) * pageCount;
        return source.Skip(rowsToSkip).Take(pageCount);
    }
}