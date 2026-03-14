namespace ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

/// <summary>
/// Aggregate root for documents; enforces creation rules and invariants.
/// </summary>
public class Document
{
    public Guid Id { get; private set; }
    public Guid FolderId { get; private set; }
    public string FileName { get; private set; } = "";
    private readonly Dictionary<string, string?> _metadata = new(StringComparer.OrdinalIgnoreCase);
    public Guid? PartnerId { get; private set; }
    public IReadOnlyDictionary<string, string?> Metadata => _metadata;
    public DateTimeOffset CreatedAt { get; private set; }

    private readonly List<DocumentVersion> _versions = [];
    public IReadOnlyList<DocumentVersion> Versions => _versions;

    public int? CurrentVersionNo => _versions.Count == 0 ? null : _versions.Max(p => p.VersionNo);

    private Document()
    {
    }

    /// <summary>
    /// Creates a new <see cref="Document"/> while validating inputs.
    /// </summary>
    /// <exception cref="ArgumentException">If <paramref name="folderId"/> is empty or <paramref name="fileName"/> is null/whitespace.</exception>
    public static Document Create(Guid folderId, string fileName, Guid? partnerId, IDictionary<string, string?>? metadata)
    {
        if (folderId == Guid.Empty)
            throw new ArgumentException("FolderId is required.");
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("FileName is required.");
        var doc = new Document
        {
            Id = Guid.NewGuid(),
            FolderId = folderId,
            FileName = fileName.Trim(),
            PartnerId = partnerId,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        if (metadata != null)
        {
            foreach (var kv in metadata)
            {
                doc._metadata[kv.Key] = kv.Value;
            }
        }

        return doc;
    }

    /// <summary>
    /// Adds a new file version to the document: computes the next version number,
    /// delegates creation to <see cref="DocumentVersion.Create"/>, stores and returns it.
    /// This method does not perform any status checks.
    /// </summary>
    /// <param name="blobUrl">Storage URL of the blob (non-empty).</param>
    /// <param name="contentType">MIME type of the blob.</param>
    /// <param name="sizeBytes">Size in bytes.</param>
    /// <param name="sha256">SHA-256 checksum of the blob.</param>
    /// <returns>The created <see cref="DocumentVersion"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when arguments are invalid; exceptions from <see cref="DocumentVersion.Create"/> are propagated.</exception>
    public DocumentVersion AddVersion(
        string blobUrl,
        string contentType,
        long sizeBytes,
        string sha256)
    {
        var nextNo = _versions.Count == 0 ? 1 : _versions.Max(v => v.VersionNo) + 1;
        var version = DocumentVersion.Create(Id, nextNo, blobUrl, contentType, sizeBytes, sha256);
        _versions.Add(version);
        return version;
    }
}