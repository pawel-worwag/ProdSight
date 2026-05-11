namespace ProdSight.Api.Modules.DocumentsModule.Application.Models;

public sealed record DocumentListItem(
    Guid Id,
    Guid FolderId,
    Guid? BusinessPartnerId,
    string FileName,
    DateTimeOffset CreatedAt);
