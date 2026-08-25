# Optimizely CMS 12 Blog Training Test

This blog is built from an empty Optimizely CMS 12 project. The supplied UI in `wwwroot` is preserved as the original asset contract; CMS/Razor replaces mock data with editor-managed content.

## Run locally

Prerequisites:

- .NET 8 SDK
- SQL Server LocalDB, or a SQL Server instance reachable from the local machine
- Optimizely Search & Navigation / Find settings

Create `appsettings.Development.json` in the project root. It must contain:

- a connection string for the local CMS database;
- the Find service URL, index and API key.

This file is gitignored because it contains sensitive values. Message the repository author directly to obtain the development configuration.

```powershell
dotnet restore
dotnet run
```

Open the HTTPS URL shown in the terminal, normally `https://localhost:5000`.

## Content and database setup

Choose one of these options to get the sample data:

1. Use the included `App_Data/TrainingTest.mdf` database by configuring its connection string in `appsettings.Development.json`. If that database cannot be attached or used locally, restore `App_Data/TrainingTest-250826.bacpac` with SQL Server Management Studio or `SqlPackage` instead.
2. Create an empty CMS database, then import `App_Data/ExportedFile.episerverdata` through the CMS Admin import tool.

Next, update the Find service URL, index and API key in `appsettings.Development.json`, then run **Find Indexing Job** in CMS Admin to populate the search index.

To recreate the blog sample content after creating the Start Page and Blog List structure, run these CMS Admin jobs in order:

1. **Seed blog content**
2. **Normalize blog tags**

The seed job reads `SeedData/blog-content.json`, creates three author profiles and 25 posts with stable keys from `job-blog-01` to `job-blog-25`. Re-running it does not create duplicates; it deliberately does not create a missing Blog List or Site Settings page.

## Completion status

| Task | Status | Completed scope |
| --- | --- | --- |
| 1 | Complete | Content types, media types, templates, blog list/detail, paging and reading time |
| 2 | Complete | TinyMCE Main body styles and reduced Intro toolbar |
| 3 | Complete | Related-post display options: Full width, Half width and Card |
| 4 | Complete | Per-site settings, Header/Footer Inherit/Override/Hidden and layout resolver |
| 5 | Complete | Content-seed and tag-normalization scheduled jobs |
| 6 | Complete | Search & Navigation conventions, computed fields and index exclusions |
| 7 | Complete | Blog search, facets, paging, site search, boosting and Best Bets |
| 8 | Complete | Author profile route, author paging and visitor-facing 404 |

## Not complete / known issues

- **The Authors link in Header/Footer is not implemented.** Author URLs are used in post teasers/details and on the Start Page; the Authors navigation item has not been added to Header/Footer.
- Search returns results only when `appsettings.Development.json` has valid Find settings and the index has been run.
- Best Bets are administrator configuration in Search & Navigation Admin. Create a rule for the `optimizely` phrase after content is indexed.

## Design notes / justifications

- **Site settings and layout:** each Start Page has one `SiteSettingsPage`. `ISiteSettingsResolver` resolves and caches it per site/language, while `IPageLayoutResolver` keeps Inherit/Override/Hidden decisions out of Razor.
- **Search indexing:** reading time, plain-text body and age in days are computed Find fields, rather than extra content-model properties. Future posts, images/videos, utility pages and InternalNotes are excluded from the index.
- **Search APIs:** Blog List uses a typed Find query because it needs `BlogPostPage` filters and facets. Site Search uses Unified Search to combine pages, blog posts and documents.
- **Ranking:** recent posts and the `optimizely` tag are boosted for text searches. `ApplyBestBets()` lets an administrator promote content without hard-coded content IDs.
- **Author URLs:** `IPartialRouter` extends a Blog List URL to `/blog/author/{slug}`. The router parses the route; the service/controller validates the profile so an invalid slug returns a site-layout HTTP 404.

## Useful URLs

- Blog list: `/en/blog/`
- Site search: `/en/search/`
- Author example: `/en/blog/author/jane-doe`
- Author page 2: `/en/blog/author/jane-doe/page-2`
