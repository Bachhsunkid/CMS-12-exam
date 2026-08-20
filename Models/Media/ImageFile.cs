using EPiServer.Framework.DataAnnotations;

namespace TrainingTest.Models.Media;

[ContentType(DisplayName = "Image file",
    Description = "Image media available for blog content",
    GUID = "4236a1fc-3ddf-47fd-b11e-a7e9ac981c2d")]
[MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png")]
public class ImageFile : ImageData;
