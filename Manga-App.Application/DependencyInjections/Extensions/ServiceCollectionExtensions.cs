
using FluentValidation;
using MangaApp.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MangaApp.Application.DependencyInjections.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMediatRApplication(this IServiceCollection services)
    {

        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssemblyContaining(typeof(ServiceCollectionExtensions));
    }

}
