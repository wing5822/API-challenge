using Amazing.Application.DTO.GraphTypes;
using Amazing.Application.Repositories;
using Amazing.Persistence.Models;
using GraphQL.Types;

namespace Amazing.Application._Queries
{
    public class ContentQueries : ObjectGraphType
    {
        private readonly IUnitOfWork _uow;

        public ContentQueries(IUnitOfWork uow)
        {
            this._uow = uow;
            this.Field<ContentGraphType>("get",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "contentId"
                }),
                resolve: this.Get);
        }

        public Content Get(ResolveFieldContext<object> context)
            => this._uow.ContentRepository.Get(context.GetArgument<int>("contentId"));
    }
}