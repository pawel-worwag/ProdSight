using ProdSight.Api.Modules.DocumentsModule.Infrastructure.Storage;

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Tests.Storage;

public class DiskFileStorageTests
{
    public static TheoryData<string> TraversalPaths => new()
    {
        "../../secret",
        "..\\..\\secret",
        "2026/05/../secret",
        "./secret",
        Path.GetFullPath("secret")
    };

    public static TheoryData<string, string> ValidPaths => new()
    {
        { "2026/05/01JVS8KJ6F7GQQZ6P7F6M7V9DA", "2026/05/01JVS8KJ6F7GQQZ6P7F6M7V9DA" },
        { "2026\\05\\file", "2026/05/file" }
    };

    [Theory]
    [MemberData(nameof(TraversalPaths))]
    public void NormalizeRelativePath_ShouldReject_PathTraversalAttempts(string path)
    {
        Assert.Throws<InvalidOperationException>(() => DiskFileStoragePathValidator.NormalizeRelativePath(path));
    }

    [Theory]
    [MemberData(nameof(ValidPaths))]
    public void NormalizeRelativePath_ShouldReturn_NormalizedRelativePath(string path, string expected)
    {
        var result = DiskFileStoragePathValidator.NormalizeRelativePath(path);

        Assert.Equal(expected, result);
    }
}
