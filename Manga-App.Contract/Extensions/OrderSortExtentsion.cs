

using MangaApp.Contract.Shares;

namespace MangaApp.Contract.Extensions;

public static class OrderSortExtentsion
{
    public static SortOrder GetSortOrder(this string orderBy)
    {
        return !string.IsNullOrWhiteSpace(orderBy) ? orderBy.ToLower().Equals("asc") ? SortOrder.Ascending : SortOrder.Descending : SortOrder.Descending;
    }
}
