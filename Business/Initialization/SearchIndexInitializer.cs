using EPiServer.Find;
using EPiServer.Find.ClientConventions;
using EPiServer.Find.Cms;
using EPiServer.Find.Cms.Conventions;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Web;
using TrainingTest.Business.Helpers;
using TrainingTest.Models.Blocks;
using TrainingTest.Models.Media;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Initialization;

/// <summary>
/// Applies the blog's Search & Navigation conventions once CMS and Find services are available.
/// See https://docs.developers.optimizely.com/content-management-system/v1.1.0-search-and-navigation/docs/index-integrated-solution
/// and https://docs.developers.optimizely.com/content-management-system/v1.1.0-search-and-navigation/docs/including-fields
/// </summary>
[InitializableModule]
[ModuleDependency(typeof(InitializationModule))]
public class SearchIndexInitializer : IInitializableModule
{
    public void Initialize(InitializationEngine context)
    {
        // Conventions must be attached to the DI-managed Find client and CMS indexer before content is indexed.
        var client = context.Locate.Advanced.GetRequiredService<IClient>();
        var contentIndexer = context.Locate.Advanced.GetRequiredService<IContentIndexer>();
        
        // Computed fields stay outside the CMS model but are available to Task 7 queries and filters.
        client.Conventions.ForInstancesOf<BlogPostPage>()
            .IncludeField(post => post.GetReadingTimeMinutes())
            .IncludeField(post => post.GetSearchableMainBody())
            .IncludeField(post => post.GetAgeInDays())
            .ExcludeField(post => post.InternalNotes);

        // A published post is not searchable until its editorial publish date is reached.
        contentIndexer.Conventions.ForInstancesOf<BlogPostPage>()
            .ShouldIndex(post => post.PublishDate.HasValue && post.PublishDate.Value <= DateTime.Now);

        // This page exists solely to host the Task 7 search interface, never as a search result.
        contentIndexer.Conventions.ForInstancesOf<SiteSearchPage>()
            .ShouldIndex(_ => false);

        // Only documents remain eligible media. Search & Navigation extracts text from PDF, DOC and DOCX by default.
        contentIndexer.Conventions.ForInstancesOf<MediaData>()
            .ShouldIndex(media => media is DocumentFile);
    }

    public void Uninitialize(InitializationEngine context)
    {
    }
}
