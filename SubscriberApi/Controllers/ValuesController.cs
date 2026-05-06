using Microsoft.AspNetCore.Mvc;
using SubscriberApi.BusinessServices;

namespace SubscriberApi.Controllers;

[ApiController]
[Route("api/loans")]
public class LoansController : ControllerBase
{
    private readonly PersistentLoanService _service;

    public LoansController(
        PersistentLoanService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(decimal amount)
    {
        await _service.AddLoan(amount);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var loans = await _service.GetAll();

        return Ok(loans);
    }
}