namespace ProdSight.Api.Shared.Exceptions;

public class NotFoundException(string message) : AppException(message, 404, "Not found");