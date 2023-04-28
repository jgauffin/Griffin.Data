using Griffin.Data;
using Griffin.Data.Configuration;
using D:\src\jgauffin\Griffin.Data\src\DemoApp\Domain.TodoTasks;

namespace D:\src\jgauffin\Griffin.Data\src\DemoApp\Data.TodoTasks.Mappings
{
    public class DocumentReviewMapping : IEntityConfigurator<DocumentReview>
    {
        public void Configure(IClassMappingConfigurator<DocumentReview> config)
        {
            config.TableName("TodoTaskDocumentReviews");
            config.Key(x => x.TaskId).AutoIncrement();
            config.Property(x => x.DocumentUrl);
            config.Property(x => x.Comment);
        }
    }
}
