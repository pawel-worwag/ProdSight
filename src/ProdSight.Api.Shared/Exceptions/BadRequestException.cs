namespace ProdSight.Api.Shared.Exceptions;

public class BadRequestException(string message) : AppException(message, 400, "Bad request");