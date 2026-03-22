namespace ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

/// <summary>
/// Aggregate root representing a folder in the 'documents' module.
/// </summary>
public class Folder
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = "";
    public string? Description { get; private set; }
    public Guid? ParentId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyList<Folder> Children { get; private set; } = new List<Folder>();
    
    private Folder() { }
    
    /// <summary>
    /// Creates a new <see cref="Folder"/> instance.
    /// </summary>
    /// <param name="name">Folder name (non-empty).</param>
    /// <param name="description">Optional description.</param>
    /// <param name="parentId">Optional parent folder id.</param>
    /// <returns>New <see cref="Folder"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public static Folder Create(string name, string? description, Guid? parentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name));

        return new Folder
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            ParentId = parentId,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
    
    /// <summary>
    /// Renames the folder.
    /// </summary>
    /// <param name="newName">New folder name (non-empty).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="newName"/> is null or whitespace.</exception>
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException(nameof(newName));

        Name = newName.Trim();
    }

    /// <summary>
    /// Replaces the folder description (allows null to clear it).
    /// </summary>
    /// <param name="newDescription">New description or null.</param>
    public void ChangeDescription(string? newDescription)
    {
        Description = newDescription?.Trim();
    }

    /// <summary>
    /// Changes the parent folder reference (allows null to make this folder a root).
    /// </summary>
    /// <param name="newParentId">New parent folder id or null.</param>
    public void ChangeParent(Guid? newParentId)
    {
        ParentId = newParentId;
    }
}