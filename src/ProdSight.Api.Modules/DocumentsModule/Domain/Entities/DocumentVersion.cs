namespace ProdSight.Api.Modules.DocumentsModule.Domain.Entities;

/// <summary>
/// Aggregate root for document versions; instances are created via <see cref="Create"/>.
/// </summary>
public class DocumentVersion
{
    public Guid Id { get; private set; }
    public Guid DocumentId { get; private set; }
    public int VersionNo { get; private set; }
    public string BlobUrl { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public string Sha256 { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    private DocumentVersion()
    {
    }

    /// <summary>
    /// Creates a new <see cref="DocumentVersion"/> with generated Id and current UTC creation time.
    /// </summary>
    /// <param name="documentId">Parent document identifier (must be non-empty).</param>
    /// <param name="versionNo">Sequential version number (must be &gt; 0).</param>
    /// <param name="blobUrl">Storage URL of the blob (non-empty).</param>
    /// <param name="contentType">MIME type of the blob (non-empty).</param>
    /// <param name="sizeBytes">Size in bytes (must be &gt; 0).</param>
    /// <param name="sha256">SHA-256 checksum of the blob (non-empty).</param>
    /// <returns>New <see cref="DocumentVersion"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when any required argument is invalid.</exception>
    public static DocumentVersion Create(Guid documentId, int versionNo, string blobUrl, string contentType,
        long sizeBytes, string sha256)
    {
        if (documentId == Guid.Empty)
            throw new ArgumentException("DocumentId is required.");
        if (versionNo <= 0)
            throw new ArgumentException("VersionNo is required.");
        if (string.IsNullOrWhiteSpace(blobUrl))
            throw new ArgumentException("BlobUrl is required.");
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("ContentType is required.");
        if (sizeBytes <= 0)
            throw new ArgumentException("SizeBytes is required.");
        if (string.IsNullOrWhiteSpace(sha256))
            throw new ArgumentException("Sha256 is required.");
        return new DocumentVersion()
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            VersionNo = versionNo,
            BlobUrl = blobUrl,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            Sha256 = sha256,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}