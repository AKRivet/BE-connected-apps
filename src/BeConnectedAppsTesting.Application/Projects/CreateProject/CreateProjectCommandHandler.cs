using BeConnectedAppsTesting.Domain.Common;
using BeConnectedAppsTesting.Domain.Entities;

namespace BeConnectedAppsTesting.Application.Projects.CreateProject;

public sealed class CreateProjectCommandHandler
{
    private readonly IProjectRepository _repository;

    public CreateProjectCommandHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<Guid>.Failure("Name is required.");

        if (string.IsNullOrWhiteSpace(command.Key))
            return Result<Guid>.Failure("Key is required.");

        if (string.IsNullOrWhiteSpace(command.Description))
            return Result<Guid>.Failure("Description is required.");

        if (await _repository.KeyExistsAsync(command.Key, cancellationToken))
            return Result<Guid>.Failure($"A project with the key '{command.Key}' already exists.");

        var project = Project.Create(command.Name.Trim(), command.Key.Trim(), command.Description.Trim());
        await _repository.AddAsync(project, cancellationToken);

        return Result<Guid>.Success(project.Id);
    }
}
