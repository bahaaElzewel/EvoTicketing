namespace EvoTicketing.DTOs.OutGoing;

public class TicketsDTO
{
    public int Id { get; set; }
    public string TicketCode { get; set; }
    public string IssuerName { get; set; } 
    public string Occasion { get; set; }
    public string Benefeciary { get; set; }
    public int Price { get; set; }
    public DateTime ValidUntil { get; set; }
}