namespace EvoTicketing.Models;

public class Ticket
{
    public int Id { get; set; }
    public string TicketCode { get; set; }
    public string IssuerName { get; set; } 
    public string Occasion { get; set; }
    public string Benefeciary { get; set; }
    public int Price { get; set; }
    public DateTime ValidUntil { get; set; }
    public byte Valid {get; set; }
    public DateTime CreatedAt {get; set; }
    public DateTime UpDatedAt { get; set; }
}