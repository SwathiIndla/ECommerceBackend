using ECommerce.Models.Domain;
using ECommerce.Models.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommerce.CustomSerializer
{
    public class CategoryDtoConverter : JsonConverter<CategoryDto>
    {
        public override CategoryDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
        public override void Write(Utf8JsonWriter writer, CategoryDto categoryDto, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("categoryId", categoryDto.CategoryId.ToString());
            writer.WriteString("categoryName", categoryDto.CategoryName);
            writer.WriteString("parentCategoryId",categoryDto.ParentCategoryId.ToString());

            if (categoryDto.ChildCategories.Count > 0)
            {
                writer.WriteStartArray("childCategories");
                foreach (var childCategory in categoryDto.ChildCategories)
                {
                    JsonSerializer.Serialize(writer, childCategory, options);
                }
                writer.WriteEndArray();
            }
            else
            {
                writer.WriteStartArray("childCategories");
                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
    }
}
