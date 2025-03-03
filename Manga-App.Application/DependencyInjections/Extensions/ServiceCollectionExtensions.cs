
using FluentValidation;
using MangaApp.Application.Background;
using MangaApp.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

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

    public static void AddQuartzJob(this IServiceCollection services) {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey(nameof(SyncMangaViewJob));
            q.AddJob<SyncMangaViewJob>(opts => opts.WithIdentity(jobKey));
            q.AddTrigger(opts => opts.ForJob(jobKey).WithSimpleSchedule(s => s.WithIntervalInMinutes(10).RepeatForever()));
        });
        services.AddQuartzHostedService();
    }

}
