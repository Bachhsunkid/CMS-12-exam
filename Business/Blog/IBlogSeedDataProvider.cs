using TrainingTest.Business.Models;

namespace TrainingTest.Business.Blog;

public interface IBlogSeedDataProvider
{
    BlogSeedData LoadSeedData();
}
