using ArzonOL.Enums;

namespace ArzonOL.Dtos.ProductDtos;

public class CreateProductDto
{
    public string? Name { get; set; }
    public long OldPrice { get; set; }
    public long NewPrice { get; set; }
    public string? VideoUrl { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public long Latitudes { get; set; }
    public long Longitudes { get; set; }
    public string? Region { get; set; }
    public string? Destrict { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? ProductCategoryApproachId { get; set; } 
    public Guid? UserId {get; set;}
}