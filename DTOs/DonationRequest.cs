using System.ComponentModel.DataAnnotations;

namespace DonationTask.DTOs;

public class DonationRequest
{
    [Required]
    public decimal? Amount { get; set; }

    [Required]
    public string? DonorName { get; set; }

    [Required]
    public bool? IsAnonymous { get; set; }
}