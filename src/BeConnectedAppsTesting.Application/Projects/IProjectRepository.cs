using BeConnectedAppsTesting.Domain.Entities;

namespace BeConnectedAppsTesting.Application.Projects;

public interface IProjectRepository
{
    Task<bool> KeyExistsAsync(string key, CancellationToken cancellationToken = default);
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default);
}
