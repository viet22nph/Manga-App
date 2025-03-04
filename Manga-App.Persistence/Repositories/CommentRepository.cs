
using MangaApp.Application.Abstraction.Repositories;
using MangaApp.Domain.Entities;

namespace MangaApp.Persistence.Repositories;

public class CommentRepository : RepositoryBase<Comment, Guid>, ICommentRepository
{
    public CommentRepository(AppDbContext context) : base(context)
    {
    }
}
