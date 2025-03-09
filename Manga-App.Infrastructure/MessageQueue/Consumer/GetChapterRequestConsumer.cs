

using MangaApp.Domain.Entities;
using MangApp.Application.Abstraction;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Chapter.Event;

namespace MangaApp.Infrastructure.MessageQueue.Consumer;

public class GetChapterRequestConsumer : IConsumer<NewChapterCreatedConsumer>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetChapterRequestConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task Consume(ConsumeContext<NewChapterCreatedConsumer> context)
    {
        var message = context.Message;
        // danh sách người dùng 
        var userFollowIds = 
            await _unitOfWork.FollowRepository
            .FindAll(x=> x.MangaId ==  message.MangaId)
            .Select(y=> y.UserId).ToListAsync();


        foreach (var userFollowId in userFollowIds)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ReceiveId = userFollowId,
                Type = Contract.Shares.Enums.NotificationType.NewChapter,
                Message = $"Truyện '{message.MangaName}' vừa ra một chương mới!",
                CreatedDate = message.CreatedAt,
                Read = false,
                SenderId = null,
                Url = $"/{message.MangaSlug}",
            };
            _unitOfWork.NotificationRepository.Add(notification);
        }
        await _unitOfWork.SaveChangesAsync();
    }
}
