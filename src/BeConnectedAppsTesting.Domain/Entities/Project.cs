namespace BeConnectedAppsTesting.Domain.Entities;

public sealed class Project
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Key { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Project() { }

    public static Project Create(string name, string key, string description) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Key = key,
            Description = description,
        };
}
