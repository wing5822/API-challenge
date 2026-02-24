using Amazing.Persistence.Models;
using GraphQL.Types;

namespace Amazing.Application.DTO.GraphTypes
{
    public class ContentGraphType : ObjectGraphType<Content>
    {
        public ContentGraphType()
        {
            this.Field(c => c.Id);
            this.Field(c => c.PostId);
            this.Field(c => c.Sort);
            this.Field<EContentTypeGraphType>("type", resolve: c => c.Source.Type);

            this.Field(c => c.Text, nullable: true);   // Text + Quote
            this.Field(c => c.ImageUrl, nullable: true);   // Image
            this.Field(c => c.Height, nullable: true);
            this.Field(c => c.Width, nullable: true);
            this.Field(c => c.VideoUrl, nullable: true);   // Video
            this.Field(c => c.Duration, nullable: true);
            this.Field(c => c.Longitude, nullable: true);  // Google Map
            this.Field(c => c.Latitude, nullable: true);
            this.Field(c => c.Author, nullable: true);  // Quote
        }
    }
}