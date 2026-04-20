namespace SubscriberApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using SubscriberApi.Models;
using SubscriberApi.Persistence;
using SubscriberApi.Requests;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly IPersistenceService _persistenceService;

    public LoansController(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    // =========================
    // GET: api/loans
    // =========================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Loan>>> GetAll(CancellationToken cancellationToken)
    {
        var loans = await _persistenceService.GetAllAsync<Loan>(cancellationToken);
        return Ok(loans);
    }

    // =========================
    // GET: api/loans/{id}
    // =========================
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Loan>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var loan = await _persistenceService.GetByIdAsync<Loan>(id, cancellationToken);

        if (loan is null)
            return NotFound();

        return Ok(loan);
    }

    // =========================
    // POST: api/loans
    // =========================
    [HttpPost]
    public async Task<ActionResult<Loan>> Create(
        [FromBody] CreateLoanRequest request,
        CancellationToken cancellationToken)
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(request.BorrowerName))
            return BadRequest("BorrowerName is required.");

        if (request.Amount <= 0)
            return BadRequest("Amount must be greater than zero.");

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            BorrowerName = request.BorrowerName,
            Amount = request.Amount,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _persistenceService.AddAsync(loan, cancellationToken);
        await _persistenceService.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    // =========================
    // PUT: api/loans/{id}
    // =========================
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateLoanRequest request,
        CancellationToken cancellationToken)
    {
        var existingLoan = await _persistenceService.GetByIdAsync<Loan>(id, cancellationToken);

        if (existingLoan is null)
            return NotFound();

        // Validation
        if (!string.IsNullOrWhiteSpace(request.BorrowerName))
            existingLoan.BorrowerName = request.BorrowerName;

        if (request.Amount.HasValue)
        {
            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            existingLoan.Amount = request.Amount.Value;
        }

        _persistenceService.Update(existingLoan);
        await _persistenceService.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    // =========================
    // DELETE: api/loans/{id}
    // =========================
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var loan = await _persistenceService.GetByIdAsync<Loan>(id, cancellationToken);

        if (loan is null)
            return NotFound();

        _persistenceService.Remove(loan);
        await _persistenceService.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}

