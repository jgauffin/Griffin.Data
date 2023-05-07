using Griffin.Data;
using Griffin.Data.Configuration;
using DemoApp.Core.TodoTasks;

namespace DemoApp.Data.TodoTasks.Mappings
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
