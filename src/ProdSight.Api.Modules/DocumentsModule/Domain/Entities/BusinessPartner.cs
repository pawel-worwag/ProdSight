namespace ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

/// <summary>
/// Represents a partner (supplier/service provider) used by the Documents module.
/// </summary>
/// <remarks>
/// This type acts as an aggregate root with a factory method <see cref="Create(string,string?)"/>.
/// </remarks>
public class BusinessPartner
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    
    private BusinessPartner() { }
    
    /// <summary>
    /// Creates a new <see cref="BusinessPartner"/> instance after basic validation.
    /// </summary>
    /// <param name="name">BusinessPartner name; must not be null, empty or whitespace.</param>
    /// <param name="address">Optional address; trimmed if provided.</param>
    /// <returns>A newly created <see cref="BusinessPartner"/> with generated Id and CreatedAt set to UTC now.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null/empty/whitespace.</exception>
    public static BusinessPartner Create(string name, string? address, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name));

        return new BusinessPartner
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Address = address?.Trim(),
            Description = description?.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Changes the partner's name after validation.
    /// </summary>
    /// <param name="newName">New name; must not be null, empty or whitespace.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="newName"/> is null/empty/whitespace.</exception>
    public void ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException(nameof(newName));
        Name = newName.Trim();
    }

    /// <summary>
    /// Updates the partner's address. Pass <c>null</c> to clear it.
    /// </summary>
    /// <param name="newAddress">New address or <c>null</c>.</param>
    public void ChangeAddress(string? newAddress)
    {
        Address = newAddress?.Trim();
    }

    /// <summary>
    /// Updates the partner's description. Pass <c>null</c> to clear it.
    /// </summary>
    /// <param name="newDescription">New description or <c>null</c>.</param>
    public void ChangeDescription(string? newDescription)
    {
        Description = newDescription?.Trim();   
    }
}