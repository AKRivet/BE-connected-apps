using BeConnectedAppsTesting.Application.Projects;
using BeConnectedAppsTesting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeConnectedAppsTesting.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> KeyExistsAsync(string key, CancellationToken cancellationToken = default) =>
        _context.Projects.AnyAsync(p => p.Key == key, cancellationToken);

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _context.Projects.AddAsync(project, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Projects.ToListAsync(cancellationToken);
}
