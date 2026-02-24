using Amazing.Application.DTO.GraphTypes;
using Amazing.Application.DTO.InputTypes;
using Amazing.Application.Extensions;
using Amazing.Application.Repositories;
using Amazing.Persistence.Enumerators;
using Amazing.Persistence.Models;
using GraphQL.Types;
using System;
using System.Net;

namespace Amazing.Application._Mutations
{
    public class ContentMutations : ObjectGraphType
    {
        private readonly IUnitOfWork _uow;

        public ContentMutations(IUnitOfWork uow)
        {
            this._uow = uow;
            this.Field<ContentGraphType>("create",
                arguments: new QueryArguments(new QueryArgument<CreateContentInputType> { Name = "request" }),
            resolve: this.Create);


            this.Field<ContentGraphType>("update",
                 arguments: new QueryArguments(new QueryArgument<UpdateContentInputType> { Name = "request" }),
                 resolve: this.Update);

            this.Field<BooleanGraphType>("delete",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "contentId"
                }),
                resolve: this.Delete);
        }

        /// <summary>
        /// Create a content
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Content Create(ResolveFieldContext<object> context)
        {
            var userId = context.UserContext().GetUserIdFromBearer();
            var request = context.GetArgument<CreateContentInputType>("request");

            var post = this._uow.PostRepository.Get(request.PostId, "Contents")
                .WhenNull()
                .Throw(HttpStatusCode.NotFound, "Post not found");

            var content = new Content
            {
                PostId = request.PostId,
                Sort = post.Contents.Count,
                CreationDate = DateTime.Now,
                Type = request.Type
            };

            switch (request.Type)
            {
                case EContentType.Text:
                    content.Text = request.Text;
                    break;
                case EContentType.Image:
                    content.ImageUrl = request.ImageUrl;
                    content.Height = request.Height;
                    content.Width = request.Width;
                    break;
                case EContentType.Video:
                    content.VideoUrl = request.VideoUrl;
                    content.Duration = request.Duration;
                    break;
                case EContentType.GoogleMap:
                    content.Longitude = request.Longitude;
                    content.Latitude = request.Latitude;
                    break;
                case EContentType.Quote:
                    content.Text = request.Text;
                    content.Author = request.Author;
                    break;
            }

            this._uow.ContentRepository.Add(content);
            return content;
        }

        public Content Update(ResolveFieldContext<object> context)
        {
            var userId = context.UserContext().GetUserIdFromBearer();
            var request = context.GetArgument<UpdateContentInputType>("request");

            var content = this._uow.ContentRepository.Get(request.ContentId)
                .WhenNull()
                .Throw(HttpStatusCode.NotFound, "Content not found");

            var post = this._uow.PostRepository.Get(content.PostId, "Blog")
                .WhenNull()
                .Throw(HttpStatusCode.NotFound, "Post not found");

            post.Blog
                .WhenConditionFailed(b => b.UserId == userId)
                .Throw(HttpStatusCode.Unauthorized, "Unauthorized");

            // clear all type-specific fields before applying new values
            content.Text = null;
            content.ImageUrl = null;
            content.Height = null;
            content.Width = null;
            content.VideoUrl = null;
            content.Duration = null;
            content.Longitude = null;
            content.Latitude = null;
            content.Author = null;

            content.Type = request.Type;

            switch (request.Type)
            {
                case EContentType.Text:
                    content.Text = request.Text;
                    break;
                case EContentType.Image:
                    content.ImageUrl = request.ImageUrl;
                    content.Height = request.Height;
                    content.Width = request.Width;
                    break;
                case EContentType.Video:
                    content.VideoUrl = request.VideoUrl;
                    content.Duration = request.Duration;
                    break;
                case EContentType.GoogleMap:
                    content.Longitude = request.Longitude;
                    content.Latitude = request.Latitude;
                    break;
                case EContentType.Quote:
                    content.Text = request.Text;
                    content.Author = request.Author;
                    break;
            }

            this._uow.ContentRepository.Update(content);
            return content;
        }

        public object Delete(ResolveFieldContext<object> context)
        {
            var userId = context.UserContext().GetUserIdFromBearer();
            var contentId = context.GetArgument<int>("contentId");

            var content = this._uow.ContentRepository.Get(contentId)
                .WhenNull()
                .Throw(HttpStatusCode.NotFound, "Content not found");

            var post = this._uow.PostRepository.Get(content.PostId, "Blog")
                .WhenNull()
                .Throw(HttpStatusCode.NotFound, "Post not found");

            post.Blog
                .WhenConditionFailed(b => b.UserId == userId)
                .Throw(HttpStatusCode.Unauthorized, "Unauthorized");

            this._uow.ContentRepository.Delete(contentId);
            return true;
        }
    }
}
