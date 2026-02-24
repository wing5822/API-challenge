using Amazing.Application.DTO.GraphTypes;
using Amazing.Persistence.Enumerators;
using GraphQL.Types;

namespace Amazing.Application.DTO.InputTypes
{
    public class CreateContentInputType : InputObjectGraphType
    {
        public int PostId { get; set; }
        public EContentType Type { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        public string VideoUrl { get; set; }
        public int? Duration { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string Author { get; set; }

        public CreateContentInputType()
        {
            this.Field<NonNullGraphType<IntGraphType>>("postId");
            this.Field<NonNullGraphType<EContentTypeGraphType>>("type");
            this.Field<StringGraphType>("text");
            this.Field<StringGraphType>("imageUrl");
            this.Field<IntGraphType>("height");
            this.Field<IntGraphType>("width");
            this.Field<StringGraphType>("videoUrl");
            this.Field<IntGraphType>("duration");
            this.Field<FloatGraphType>("longitude");
            this.Field<FloatGraphType>("latitude");
            this.Field<StringGraphType>("author");
        }
    }
}