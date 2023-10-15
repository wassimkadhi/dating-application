namespace API;

public class UserParams
{   
    private const int MaxePageSize =50;
    public int PageNumber { get; set; } =1 ; 

    public int _pageSize { get; set; } =10 ;

    

    
    public int PageSize
    {
        get =>_pageSize;
        set =>_pageSize=(value > MaxePageSize) ? MaxePageSize:value ;
    }

    public string CurrentUsername { get; set; }
    public string Gender { get; set; }
    public int MinAge { get; set; }=18 ;
    public int MaxAge { get; set; } =100 ;
    public string OrderBy { get; set; } ="lastActive" ;
    
    


}
