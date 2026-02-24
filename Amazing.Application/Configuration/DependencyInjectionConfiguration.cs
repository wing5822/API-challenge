using Amazing.Application._Mutations;
using Amazing.Application._Queries;
using Amazing.Application.Context;
using Amazing.Application.DTO.GraphTypes;
using Amazing.Application.DTO.InputTypes;
using Amazing.Application.Repositories;
using Amazing.Application.Schemas;
using Amazing.Application.Schemas.Logged;
using Amazing.Application.Schemas.Public;
using GraphQL;
using GraphQL.Types.Relay;
using Microsoft.Extensions.DependencyInjection;

namespace Amazing.Application.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void ConfigureQueriesMutations(IServiceCollection services)
        {
            services.AddTransient<PublicMutations>();
            services.AddTransient<PublicQueries>();

            services.AddTransient<LoggedQueries>();
            services.AddTransient<LoggedMutations>();

            services.AddTransient<MutationCollection>();
            services.AddTransient<QueryCollection>();

            services.AddTransient<BlogMutations>();
            services.AddTransient<BlogQueries>();
            services.AddTransient<UserMutations>();
            services.AddTransient<UserQueries>();
            services.AddTransient<PostMutations>();
            services.AddTransient<PostQueries>();
            services.AddTransient<ContentMutations>();
            services.AddTransient<ContentQueries>();

        }

        public static void ConfigureEnumType(IServiceCollection services)
        {
        }

        public static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IBlogRepository, BlogRepository>();
            services.AddTransient<IContentRepository, ContentRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
        }

        public static void ConfigureGraphType(IServiceCollection services)
        {
            services.AddTransient(typeof(ConnectionType<>));
            services.AddTransient(typeof(EdgeType<>));
            services.AddTransient<PageInfoType>();

            services.AddTransient<LoginUserInputType>();
            services.AddTransient<RegisterUserInputType>();
            services.AddTransient<CreateBlogInputType>();
            services.AddTransient<CreateContentInputType>();
            services.AddTransient<CreatePostInputType>();

            services.AddTransient<BlogGraphType>();
            services.AddTransient<UserGraphType>();
            services.AddTransient<PostGraphType>();
            services.AddTransient<ContentGraphType>();
            services.AddTransient<EContentTypeGraphType>();
            services.AddTransient<UpdateContentInputType>();
        }

        public static void ConfigureSchema(IServiceCollection services)
        {
            services.AddTransient<ISchemaFactory, SchemaFactory>();
            services.AddTransient<ISchemaCollection, SchemaCollection>();

            var sp = services.BuildServiceProvider();
            services.AddSingleton<IPublicSchema>(new PublicSchema(new FuncDependencyResolver(type => sp.GetService(type))));
            services.AddSingleton<ILoggedSchema>(new LoggedSchema(new FuncDependencyResolver(type => sp.GetService(type))));
        }

        public static void ConfigureAmazing(this IServiceCollection services)
        {
            services.AddTransient<AmazingRequestContext>();

            ConfigureGraphType(services);
            ConfigureQueriesMutations(services);
            ConfigureRepositories(services);
            ConfigureEnumType(services);
            ConfigureSchema(services);

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
        }
    }
}