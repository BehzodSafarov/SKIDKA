using ArzonOL.Entities;
using ArzonOL.Enums;
namespace ArzonOL.Models;

public class ProductModel
{
    public Guid Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Name { get; set; }
    public long OldPrice { get; set; }
    public long NewPrice { get; set; }
    public double? Discount { get; set; }
    public string? ProductMedias {get; set; }
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
    public ICollection<ProductVoterModel>? Voters { get; set; }
    public long? BoughtCount { get; set; }
    public float? VotesResult {get; set;}
}