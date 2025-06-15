using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EvoTicketing.Data;
using EvoTicketing.DTOs.Incoming;
using EvoTicketing.DTOs.OutGoing;
using EvoTicketing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EvoTicketing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly EvoTicketingDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(EvoTicketingDbContext context, IMapper mapper, ILogger<TicketsController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("AllTickets")]
    public async Task<IActionResult> Tickets()
    {
        // var tickets = await _context.tickets.FirstOrDefaultAsync();
        var ticketsQuery = _context.tickets.AsQueryable();
        var projectedTickets = ticketsQuery.ProjectTo<TicketsDTO>(_mapper.ConfigurationProvider);
        var tickets = await projectedTickets.ToListAsync();
        return Ok(tickets);
    }

    [HttpGet("FindTicketId/{ticketId}")]
    public async Task<IActionResult> FindTicketId(int ticketId)
    {
        var ticketQuery = _context.tickets.AsQueryable();
        var projectedTicket = ticketQuery.ProjectTo<TicketsDTO>(_mapper.ConfigurationProvider);
        var ticket = await projectedTicket.Where(x => x.Id == ticketId).FirstOrDefaultAsync();
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
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to save the ticket: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    [HttpPost("GetTicketsFromRabbitMQ")]
    public async Task<IActionResult> GetTickersFromRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "bahaa",
            Password = "bahaa",
            VirtualHost = "/",
        };

        var conn = await factory.CreateConnectionAsync();

        var channel = await conn.CreateChannelAsync();

        await channel.QueueDeclareAsync("ticketsIssuing", durable: true, exclusive: false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        
        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Received message: {Message}", message);

            // Here you can process the message as needed
            // For example, you could deserialize it and save it to the database

            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);

        };

        await channel.BasicConsumeAsync("ticketsIssuing", true, consumer);

        return Ok();
    }
}