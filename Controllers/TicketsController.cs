using System.Net;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EvoTicketing.Data;
using EvoTicketing.DTOs.Incoming;
using EvoTicketing.DTOs.OutGoing;
using EvoTicketing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvoTicketing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly EvoTicketingDbContext _context;
    private readonly IMapper _mapper;
    
    public TicketsController(EvoTicketingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper  = mapper;
    }
    
    [HttpGet("AllTickets")]
    public async Task<IActionResult> Tickets()
    {
        // var tickets = await _context.tickets.FirstOrDefaultAsync();
        var ticketsQuery     = _context.tickets.AsQueryable();
        var projectedTickets = ticketsQuery.ProjectTo<TicketsDTO>(_mapper.ConfigurationProvider);
        var tickets          = await projectedTickets.ToListAsync();
        return Ok(tickets);
    }

    [HttpGet("FindTicketId/{ticketId}")]
    public async Task<IActionResult> FindTicketId(int ticketId)
    {
        var ticketQuery     = _context.tickets.AsQueryable();
        var projectedTicket = ticketQuery.ProjectTo<TicketsDTO>(_mapper.ConfigurationProvider);
        var ticket          = await projectedTicket.Where(x => x.Id == ticketId).FirstOrDefaultAsync();
        return Ok(ticket);
    }

    [HttpPost("CreateTicket")]
    public async Task<IActionResult> CreateTicket([FromBody] NewTicketDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Bad request");
        
        var _newTicket = _mapper.Map<Ticket>(request);
        try 
        {
            await _context.AddAsync(_newTicket);
            await _context.SaveChangesAsync();
            var _mappedOut = _mapper.Map<TicketsDTO>(_newTicket);
            return Ok(_mappedOut);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to save the ticket: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
}