
namespace MangaApp.Contract.Shares;

public class Pagination<T>
{
    protected Pagination(List<T> items, int pageIndex, int pageSize, int totalCount)
    {
        Data = items;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
    public List<T> Data { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasNextPage => PageIndex * PageSize < TotalCount;
    public bool HasPreviousPage => PageIndex > 1;
    public static Pagination<T> Create(List<T> items, int pageIndex, int pageSize, int totalCount)
    => new(items, pageIndex, pageSize, totalCount);
}