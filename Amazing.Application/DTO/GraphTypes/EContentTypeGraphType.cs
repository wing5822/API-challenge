using System;
using System.Collections.Generic;
using System.Text;
using Amazing.Persistence.Enumerators;
using GraphQL.Types;

namespace Amazing.Application.DTO.GraphTypes
{
    public class EContentTypeGraphType : EnumerationGraphType
    {
        public EContentTypeGraphType()
        {
            this.Name = "ContentType";
            this.AddValue("TEXT", "Plain text", EContentType.Text);
            this.AddValue("IMAGE", "Image", EContentType.Image);
            this.AddValue("VIDEO", "Video", EContentType.Video);
            this.AddValue("GOOGLE_MAP", "Google Map", EContentType.GoogleMap);
            this.AddValue("QUOTE", "Quote", EContentType.Quote);
        }
    }
}
