using EPiServer.Framework.DataAnnotations;

namespace TrainingTest.Models.Media;

[ContentType(
    DisplayName = "Document file",
    Description = "PDF and Word documents available for blog content.",
    GUID = "75f52d42-27be-4f12-9634-33dc60b7f2b1")]
[MediaDescriptor(ExtensionString = "pdf,doc,docx")]
public class DocumentFile : MediaData;
