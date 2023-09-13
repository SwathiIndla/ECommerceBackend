using ECommerce.Models.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommerce.CustomSerializer
{
    public class CategoryDtoListConverter : JsonConverter<List<CategoryDto>>
    {
        public override List<CategoryDto>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, List<CategoryDto> categoryDtoList, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteStartArray("categories");
            foreach(var  categoryDto in categoryDtoList)
            {
                JsonSerializer.Serialize(writer, categoryDto, options);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
