namespace DonationTask.DTOs;

public class DonationResponse
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string DonorName { get; set; }=null!;
    public bool IsAnonymous { get; set; }
    public DateTime CreatedAt { get; set; }
}