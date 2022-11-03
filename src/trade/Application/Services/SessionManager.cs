using Data.Contexts;
using Domain.Enums;
using Domain.Models;
using Timer = System.Timers.Timer;

namespace Application.Services;

public class SessionManager
{
    private readonly AppDbContext _context;
    public SessionManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task AbortAsync(long sessionId, CancellationToken token = default)
    {
        var session = await _context.Sessions.FindAsync(new object?[] { sessionId }, cancellationToken: token);
        if (session is null) throw new ArgumentException($"Session with id {sessionId} is not found");
        if (session.SessionStatus == SessionStatus.Started)
        {
            session.SessionStatus = SessionStatus.Aborted;
            await _context.SaveChangesAsync(token);
        }
    }
}