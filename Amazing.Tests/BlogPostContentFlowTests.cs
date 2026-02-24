using Amazing.Tests.Helpers;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// Test suite: A user can create multiple blogs; a blog has multiple posts; a post has a list of content.
///
/// Prerequisites:
///   - The Amazing API must be running at http://localhost:5000
///   - The PostgreSQL database must be accessible and migrated
///
/// All tests register a fresh user (unique email per test) so they are independent.
/// </summary>
namespace Amazing.Tests
{
    public class BlogPostContentFlowTests
    {
        // ─────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────

        private static string UniqueEmail() => $"test_{Guid.NewGuid():N}@example.com";

        /// <summary>Registers a new user and returns a JWT bearer token.</summary>
        private static async Task<string> RegisterAndLogin(string email, string password = "Password123!")
        {
            var pub = new GraphQLClient();

            await pub.SendAsync($@"
                mutation {{
                    registerUser(request: {{
                        firstname: ""Test""
                        lastname:  ""User""
                        email:     ""{email}""
                        password:  ""{password}""
                    }})
                }}");

            var loginResult = await pub.SendAsync($@"
                query {{
                    login(request: {{ email: ""{email}"", password: ""{password}"" }})
                }}");

            return loginResult["data"]!["login"]!.Value<string>();
        }

        // ─────────────────────────────────────────────────────────────
        // Happy-path tests
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task User_CanCreate_MultipleBlogs()
        {
            // Arrange
            var email = UniqueEmail();
            var token = await RegisterAndLogin(email);
            var logged = new GraphQLClient(token);

            // Act – create two separate blogs for the same user
            var blog1Result = await logged.SendAsync(@"
                mutation {
                    blog {
                        create(request: { name: ""Blog One"", url: ""https://blog-one.example.com"" }) {
                            id name url userId
                        }
                    }
                }");

            var blog2Result = await logged.SendAsync(@"
                mutation {
                    blog {
                        create(request: { name: ""Blog Two"", url: ""https://blog-two.example.com"" }) {
                            id name url userId
                        }
                    }
                }");

            // Assert
            blog1Result["errors"].Should().BeNull("creating blog 1 should succeed");
            blog2Result["errors"].Should().BeNull("creating blog 2 should succeed");

            var blog1 = blog1Result["data"]!["blog"]!["create"]!;
            var blog2 = blog2Result["data"]!["blog"]!["create"]!;

            blog1["name"]!.Value<string>().Should().Be("Blog One");
            blog2["name"]!.Value<string>().Should().Be("Blog Two");
            blog1["id"]!.Value<int>().Should().BePositive();
            blog2["id"]!.Value<int>().Should().BePositive();
            blog1["id"]!.Value<int>().Should().NotBe(blog2["id"]!.Value<int>(),
                "each blog must get its own id");
        }

        [Fact]
        public async Task Blog_CanHave_MultiplePosts()
        {
            // Arrange
            var email = UniqueEmail();
            var token = await RegisterAndLogin(email);
            var logged = new GraphQLClient(token);

            var blogResult = await logged.SendAsync(@"
                mutation {
                    blog {
                        create(request: { name: ""My Blog"", url: ""https://my-blog.example.com"" }) {
                            id
                        }
                    }
                }");
            var blogId = blogResult["data"]!["blog"]!["create"]!["id"]!.Value<int>();

            // Act – create two posts inside the same blog
            var post1Result = await logged.SendAsync($@"
                mutation {{
                    post {{
                        create(request: {{
                            title:           ""First Post""
                            releaseDateTime: ""2026-06-01T12:00:00Z""
                            blogId:          {blogId}
                        }}) {{
                            id title blogId
                        }}
                    }}
                }}");

            var post2Result = await logged.SendAsync($@"
                mutation {{
                    post {{
                        create(request: {{
                            title:           ""Second Post""
                            releaseDateTime: ""2026-07-01T12:00:00Z""
                            blogId:          {blogId}
                        }}) {{
                            id title blogId
                        }}
                    }}
                }}");

            // Assert
            post1Result["errors"].Should().BeNull("creating post 1 should succeed");
            post2Result["errors"].Should().BeNull("creating post 2 should succeed");

            var post1 = post1Result["data"]!["post"]!["create"]!;
            var post2 = post2Result["data"]!["post"]!["create"]!;

            post1["blogId"]!.Value<int>().Should().Be(blogId);
            post2["blogId"]!.Value<int>().Should().Be(blogId);
            post1["id"]!.Value<int>().Should().NotBe(post2["id"]!.Value<int>(),
                "each post must get its own id");
        }

        [Fact]
        public async Task Post_HasOrderedListOf_Contents()
        {
            // Arrange
            var email = UniqueEmail();
            var token = await RegisterAndLogin(email);
            var logged = new GraphQLClient(token);

            var blogResult = await logged.SendAsync(@"
                mutation {
                    blog {
                        create(request: { name: ""Content Blog"", url: ""https://content-blog.example.com"" }) {
                            id
                        }
                    }
                }");
            var blogId = blogResult["data"]!["blog"]!["create"]!["id"]!.Value<int>();

            var postResult = await logged.SendAsync($@"
                mutation {{
                    post {{
                        create(request: {{
                            title:           ""A Post With Contents""
                            releaseDateTime: ""2026-08-01T00:00:00Z""
                            blogId:          {blogId}
                        }}) {{
                            id
                        }}
                    }}
                }}");
            var postId = postResult["data"]!["post"]!["create"]!["id"]!.Value<int>();

            // Act – add three content items to the post
            var c1 = await logged.SendAsync($@"
                mutation {{
                    content {{
                        create(request: {{ postId: {postId}, text: ""Paragraph one."" }}) {{
                            id text sort
                        }}
                    }}
                }}");

            var c2 = await logged.SendAsync($@"
                mutation {{
                    content {{
                        create(request: {{ postId: {postId}, text: ""Paragraph two."" }}) {{
                            id text sort
                        }}
                    }}
                }}");

            var c3 = await logged.SendAsync($@"
                mutation {{
                    content {{
                        create(request: {{ postId: {postId}, text: ""Paragraph three."" }}) {{
                            id text sort
                        }}
                    }}
                }}");

            // Assert
            c1["errors"].Should().BeNull();
            c2["errors"].Should().BeNull();
            c3["errors"].Should().BeNull();

            c1["data"]!["content"]!["create"]!["sort"]!.Value<int>().Should().Be(0, "first content gets sort=0");
            c2["data"]!["content"]!["create"]!["sort"]!.Value<int>().Should().Be(1, "second content gets sort=1");
            c3["data"]!["content"]!["create"]!["sort"]!.Value<int>().Should().Be(2, "third content gets sort=2");
        }

        [Fact]
        public async Task FullHierarchy_CanBeQueried()
        {
            // Arrange – build the full user → blog → post → content tree
            var email = UniqueEmail();
            var token = await RegisterAndLogin(email);
            var logged = new GraphQLClient(token);

            var blogResult = await logged.SendAsync(@"
                mutation {
                    blog {
                        create(request: { name: ""My Hierarchy Blog"", url: ""https://hierarchy.example.com"" }) {
                            id
                        }
                    }
                }");
            var blogId = blogResult["data"]!["blog"]!["create"]!["id"]!.Value<int>();

            var post1Result = await logged.SendAsync($@"
                mutation {{
                    post {{
                        create(request: {{ title: ""Post A"", releaseDateTime: ""2026-01-01T00:00:00Z"", blogId: {blogId} }}) {{
                            id
                        }}
                    }}
                }}");
            var post1Id = post1Result["data"]!["post"]!["create"]!["id"]!.Value<int>();

            var post2Result = await logged.SendAsync($@"
                mutation {{
                    post {{
                        create(request: {{ title: ""Post B"", releaseDateTime: ""2026-02-01T00:00:00Z"", blogId: {blogId} }}) {{
                            id
                        }}
                    }}
                }}");
            var post2Id = post2Result["data"]!["post"]!["create"]!["id"]!.Value<int>();

            await logged.SendAsync($@"mutation {{ content {{ create(request: {{ postId: {post1Id}, text: ""Post A – section 1"" }}) {{ id }} }} }}");
            await logged.SendAsync($@"mutation {{ content {{ create(request: {{ postId: {post1Id}, text: ""Post A – section 2"" }}) {{ id }} }} }}");
            await logged.SendAsync($@"mutation {{ content {{ create(request: {{ postId: {post2Id}, text: ""Post B – only section"" }}) {{ id }} }} }}");

            // Act – query the full blog hierarchy
            var queryResult = await logged.SendAsync($@"
                query {{
                    blog {{
                        get(blogId: {blogId}) {{
                            id
                            name
                            posts {{
                                id
                                title
                                contents {{
                                    id
                                    text
                                    sort
                                }}
                            }}
                        }}
                    }}
                }}");

            // Assert
            queryResult["errors"].Should().BeNull("hierarchy query should succeed");

            var blog = queryResult["data"]!["blog"]!["get"]!;
            blog["name"]!.Value<string>().Should().Be("My Hierarchy Blog");

            var posts = blog["posts"]! as JArray;
            posts.Should().HaveCount(2, "the blog was given two posts");

            var postA = posts!.Cast<JToken>().First(p => p["title"]!.Value<string>() == "Post A");
            var postB = posts!.Cast<JToken>().First(p => p["title"]!.Value<string>() == "Post B");

            (postA["contents"] as JArray).Should().HaveCount(2, "Post A has two content items");
            (postB["contents"] as JArray).Should().HaveCount(1, "Post B has one content item");
        }

        [Fact]
        public async Task DeleteBlog_RemovesIt_FromOwner()
        {
            // Arrange
            var email = UniqueEmail();
            var token = await RegisterAndLogin(email);
            var logged = new GraphQLClient(token);

            var createResult = await logged.SendAsync(@"
                mutation {
                    blog {
                        create(request: { name: ""To Delete"", url: ""https://delete-me.example.com"" }) {
                            id
                        }
                    }
                }");
            var blogId = createResult["data"]!["blog"]!["create"]!["id"]!.Value<int>();

            // Act
            var deleteResult = await logged.SendAsync($@"
                mutation {{
                    blog {{
                        delete(blogId: {blogId})
                    }}
                }}");

            // Assert
            deleteResult["errors"].Should().BeNull();
            deleteResult["data"]!["blog"]!["delete"]!.Value<bool>().Should().BeTrue();
        }

        // ─────────────────────────────────────────────────────────────
        // Error / authorization tests
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsError()
        {
            // Arrange
            var email = UniqueEmail();
            var pub = new GraphQLClient();

            var mutation = $@"
                mutation {{
                    registerUser(request: {{
                        firstname: ""Test"" lastname: ""User""
                        email: ""{email}"" password: ""Password123!""
                    }})
                }}";

            await pub.SendAsync(mutation); // first registration succeeds

            // Act – same email a second time
            var result = await pub.SendAsync(mutation);

            // Assert
            var errors = result["errors"] as JArray;
            errors.Should().NotBeNullOrEmpty("duplicate email must be rejected");
            errors![0]["message"]!.Value<string>().Should().Contain("Email already used");
        }

        [Fact]
        public async Task Login_WithWrongPassword_ReturnsError()
        {
            // Arrange – register, then try to log in with bad credentials
            var email = UniqueEmail();
            var pub = new GraphQLClient();

            await pub.SendAsync($@"
                mutation {{
                    registerUser(request: {{
                        firstname: ""Test"" lastname: ""User""
                        email: ""{email}"" password: ""CorrectPass1""
                    }})
                }}");

            // Act
            var result = await pub.SendAsync($@"
                query {{
                    login(request: {{ email: ""{email}"", password: ""WrongPass!"" }})
                }}");

            // Assert
            var errors = result["errors"] as JArray;
            errors.Should().NotBeNullOrEmpty("wrong password must be rejected");
            errors![0]["message"]!.Value<string>().Should().Contain("User not found");
        }

        [Fact]
        public async Task CreateBlog_WithoutAuthentication_IsRejected()
        {
            // The public schema has no blog mutation field at all
            var pub = new GraphQLClient(); // no token

            var result = await pub.SendAsync(@"
                mutation {
                    blog {
                        create(request: { name: ""Ghost"", url: ""https://ghost.example.com"" }) {
                            id
                        }
                    }
                }");

            // GraphQL will return an error because 'blog' is not a field on the public mutation type
            var errors = result["errors"] as JArray;
            errors.Should().NotBeNullOrEmpty("unauthenticated users cannot create blogs");
        }

        [Fact]
        public async Task CreatePost_ForAnotherUsersBlog_IsUnauthorized()
        {
            // Arrange – user A creates a blog
            var emailA = UniqueEmail();
            var tokenA = await RegisterAndLogin(emailA);
            var loggedA = new GraphQLClient(tokenA);

            var blogResult = await loggedA.SendAsync(@"
                mutation {
                    blog {
                        create(request: { name: ""User A Blog"", url: ""https://user-a.example.com"" }) {
                            id
                        }
                    }
                }");
            var blogId = blogResult["data"]!["blog"]!["create"]!["id"]!.Value<int>();

            // User B tries to add a post to user A's blog
            var emailB = UniqueEmail();
            var tokenB = await RegisterAndLogin(emailB);
            var loggedB = new GraphQLClient(tokenB);

            // Act
            var result = await loggedB.SendAsync($@"
                mutation {{
                    post {{
                        create(request: {{
                            title:           ""Hijacked Post""
                            releaseDateTime: ""2026-01-01T00:00:00Z""
                            blogId:          {blogId}
                        }}) {{
                            id
                        }}
                    }}
                }}");

            // Assert
            var errors = result["errors"] as JArray;
            errors.Should().NotBeNullOrEmpty("a user may not post to another user's blog");
            errors![0]["message"]!.Value<string>().Should().Contain("Unauthorized");
        }

        [Fact]
        public async Task CreateContent_ForNonExistentPost_IsNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin(UniqueEmail());
            var logged = new GraphQLClient(token);

            // Act – use a post id that cannot exist
            var result = await logged.SendAsync(@"
                mutation {
                    content {
                        create(request: { postId: 999999999, text: ""Ghost text"" }) {
                            id
                        }
                    }
                }");

            // Assert
            var errors = result["errors"] as JArray;
            errors.Should().NotBeNullOrEmpty("creating content for a missing post must be rejected");
            errors![0]["message"]!.Value<string>().Should().Contain("Post not found");
        }

        [Fact]
        public async Task DeleteBlog_BelongingToAnotherUser_IsUnauthorized()
        {
            // Arrange – user A creates a blog
            var emailA = UniqueEmail();
            var tokenA = await RegisterAndLogin(emailA);
            var loggedA = new GraphQLClient(tokenA);

            var blogResult = await loggedA.SendAsync(@"
                mutation {
                    blog {
                        create(request: { name: ""User A Blog"", url: ""https://user-a-del.example.com"" }) {
                            id
                        }
                    }
                }");
            var blogId = blogResult["data"]!["blog"]!["create"]!["id"]!.Value<int>();

            // User B tries to delete user A's blog
            var tokenB = await RegisterAndLogin(UniqueEmail());
            var loggedB = new GraphQLClient(tokenB);

            // Act
            var result = await loggedB.SendAsync($@"
                mutation {{
                    blog {{
                        delete(blogId: {blogId})
                    }}
                }}");

            // Assert
            var errors = result["errors"] as JArray;
            errors.Should().NotBeNullOrEmpty("a user cannot delete another user's blog");
            errors![0]["message"]!.Value<string>().Should().Contain("Unauthorized");
        }
    }
}
