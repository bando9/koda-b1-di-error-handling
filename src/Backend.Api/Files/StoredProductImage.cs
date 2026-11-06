namespace Backend.Api.Files;

public sealed record StoredProductImage
(
    string StoredFileName,
    long SizeInBytes,
    string ContentType
);