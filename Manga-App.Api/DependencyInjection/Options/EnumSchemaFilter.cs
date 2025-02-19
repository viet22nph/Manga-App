using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace MangaApp.Api.DependencyInjection.Options;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum = Enum.GetNames(context.Type)
                .Select(name => new Microsoft.OpenApi.Any.OpenApiString(name))
                .Cast<Microsoft.OpenApi.Any.IOpenApiAny>() // ⚡ Explicit cast
                .ToList();
            schema.Type = "string"; // Hiển thị dạng string thay vì số
        }
    }
}