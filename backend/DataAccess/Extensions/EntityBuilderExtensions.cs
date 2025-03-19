using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess
{
    public static class EntityBuilderExtensions
    {
        public static EntityTypeBuilder HasConstructor(this EntityTypeBuilder builder, params string[] types)
        {
            ArgumentNullException.ThrowIfNull(types);
            
            if (types.Length == 0)
                return builder;

            var constructorBindingData = builder.Metadata.GetAnnotations()
                .FirstOrDefault(a => a.Name == "ConstructorBinding");

            if (constructorBindingData != null)
                builder.Metadata.RemoveAnnotation("ConstructorBinding");

            builder.Metadata.AddAnnotation("ConstructorBinding", types);

            return builder;
        }
    }
} 