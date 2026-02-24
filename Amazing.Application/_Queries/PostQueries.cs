using Amazing.Application.DTO.GraphTypes;
using Amazing.Application.Repositories;
using Amazing.Persistence.Models;
using GraphQL.Types;

namespace Amazing.Application._Queries
{
    public class PostQueries : ObjectGraphType
    {

        private readonly IUnitOfWork _uow;

        public PostQueries(IUnitOfWork uow)
        {
            this._uow = uow;
            this.Field<PostGraphType>("get",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "postId" }),
                resolve: this.Get);
        }

        public Post Get(ResolveFieldContext<object> context)
            => this._uow.PostRepository.Get(context.GetArgument<int>("postId"));
    }
}
