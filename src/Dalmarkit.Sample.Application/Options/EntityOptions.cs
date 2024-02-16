namespace Dalmarkit.Sample.Application.Options;

public class EntityOptions
{
    public string? ImageS3BucketName { get; set; }
    public string? ImageRootFolderName { get; set; }
    public string? ImageCloudFrontDistributionId { get; set; }
    public ICollection<string>? SupportedImageContentTypes { get; set; }
    public ICollection<string>? SupportedImageFileExtensions { get; set; }
}
