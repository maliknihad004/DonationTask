using DonationTask.Data;
using DonationTask.DTOs;
using DonationTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonationTask.Controllers;

[ApiController]
[Route("api/donations")]
public class DonationsController : ControllerBase
{
    private readonly DonationDbContext _context;

    public DonationsController(DonationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1)
    {
        if (page < 1)
            return BadRequest("Invalid page number.");

        int totalDonations = await _context.Donations.CountAsync();

        int pageSize = Math.Min(200, Math.Max(1,
            (int)Math.Ceiling(Math.Sqrt(totalDonations))));

        int totalPages = (int)Math.Ceiling(
            (double)totalDonations / pageSize);

        if (page > Math.Max(1, totalPages))
            return BadRequest("Page does not exist.");

        var donations = await _context.Donations
            .OrderBy(d => d.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            TotalDonations = totalDonations,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize,
            Donations = donations.Select(ToResponse).ToList()
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DonationResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid donation ID.");

        var donation = await _context.Donations.FindAsync(id);

        if (donation == null)
            return NotFound();

        return Ok(ToResponse(donation));
    }

    [HttpPost]
    [ProducesResponseType(typeof(DonationResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create(DonationRequest request)
    {
        if (request.Amount <= 0)
            return BadRequest("Donation amount must be greater than zero.");

        var donation = new Donation
        {
            Amount = request.Amount!.Value,
            DonorName = request.DonorName!,
            IsAnonymous = request.IsAnonymous!.Value,
            CreatedAt = DateTime.UtcNow
        };

        _context.Donations.Add(donation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = donation.Id },
            ToResponse(donation)
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DonationResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(
        int id, DonationRequest request)
    {
        if (id <= 0)
            return BadRequest("Invalid donation ID.");

        if (request.Amount <= 0)
            return BadRequest("Donation amount must be greater than zero.");

        var donation = await _context.Donations.FindAsync(id);

        if (donation == null)
            return NotFound();

        donation.Amount = request.Amount!.Value;
        donation.DonorName = request.DonorName!;
        donation.IsAnonymous = request.IsAnonymous!.Value;

        await _context.SaveChangesAsync();

        return Ok(ToResponse(donation));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid donation ID.");

        var donation = await _context.Donations.FindAsync(id);

        if (donation == null)
            return NotFound();

        _context.Donations.Remove(donation);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static DonationResponse ToResponse(Donation donation)
    {
        var response = new DonationResponse
        {
            Id = donation.Id,
            Amount = donation.Amount,
            IsAnonymous = donation.IsAnonymous,
            CreatedAt = donation.CreatedAt
        };

        if (donation.IsAnonymous)
        {
            response.DonorName = "Anonymous";
        }
        else
        {
            response.DonorName = donation.DonorName;
        }

        return response;
    }
}