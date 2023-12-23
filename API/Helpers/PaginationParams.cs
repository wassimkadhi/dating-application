namespace API;

public class PaginationParams 
{
 private const int MaxePageSize =50;
    public int PageNumber { get; set; } =1 ; 

    public int _pageSize { get; set; } =10 ;
    public int PageSize
    {
        get =>_pageSize;
        set =>_pageSize=(value > MaxePageSize) ? MaxePageSize:value ;
    }
}
