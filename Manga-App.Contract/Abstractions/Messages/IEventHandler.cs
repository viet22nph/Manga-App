
using MangaApp.Contract.Abstractions.Messages;
using MediatR;

namespace Manga_App.Contract.Abstractions.Messages;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
}
