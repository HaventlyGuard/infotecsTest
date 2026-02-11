using System.ComponentModel.DataAnnotations;

namespace InfotecsBackend.Models.DTO.Pagination;

public class PageFilter
{

    public class PaginationFilter
    {
        [Range(1, 1000)] 
        public int elementsLimit { get; set; } = 20;
        
        [Range(0, int.MaxValue)] 
        public int Offset { get; set; } = 0;
        
        public SortType SortDirection { get; set; } = SortType.Descending;
    }
}