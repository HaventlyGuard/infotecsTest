namespace InfotecsBackend.Models.DTO.Pagination;

public class Page<T>
{
    public Page(ICollection<T> items, int elementCount)
    {

    Items= items;
    TotalItems = elementCount;

    }
    public IEnumerable<T> Items { get; set; }
    public int TotalItems { get; set; }
}