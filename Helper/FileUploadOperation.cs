using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Meshwark.Helper
{
    public class FileUploadOperation : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileUploadParams = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile));

            if (fileUploadParams.Any())
            {
                operation.Parameters.Clear();
                foreach (var param in fileUploadParams)
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = param.Name,
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema { Type = "string", Format = "binary" }
                    });
                }
            }
        }
    }
}
