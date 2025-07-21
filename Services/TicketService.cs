using AutoMapper;
using EvoTicketing.Data;
using EvoTicketingGRPC;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace EvoTicketing.Services;

public class TicketService : TicketingService.TicketingServiceBase
{
    private readonly EvoTicketingDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TicketService> _logger;

    public TicketService(EvoTicketingDbContext context, IMapper mapper, ILogger<TicketService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public override async Task<TicketResponse> Tickets(TicketRequest request, ServerCallContext context)
    {
        var ticketsQuery = _context.tickets.AsQueryable();
        var tickets = await ticketsQuery.ToListAsync();

        var ticketDtos = tickets.Select(ticket => _mapper.Map<Ticket>(ticket));

        var response = new TicketResponse();
        response.Ticket.AddRange(ticketDtos);

        return await Task.FromResult(response);
    }
}