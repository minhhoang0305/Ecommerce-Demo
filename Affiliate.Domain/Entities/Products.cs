public class Products
{
    public Guid Id {get; set;}
    public string Name {get; set;} = default!;
    public decimal Price {get; set;}
    public string Description {get; set;} = default!;
    public int Stock {get; set;}
    public int Status {get; set;} // 0: Inactive, 1: Active
    public bool IsDeleted {get; set;} = false;
    
}