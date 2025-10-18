using Grades.Application.Interfaces;
using Grades.Infrastructure.Persistence;
using Grades.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grades.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<GradesDbContext>(opt =>
        {
            var cs = config.GetConnectionString("GradesDb");
            opt.UseSqlServer(cs);
        });

        services.AddScoped<IQuestionService, QuestionService>();

        services.AddScoped<IReportService, ReportService>();
        return services;
    }
}
