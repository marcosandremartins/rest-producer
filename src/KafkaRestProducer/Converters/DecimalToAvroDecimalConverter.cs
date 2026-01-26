namespace KafkaRestProducer.Converters
{
    using System;
    using Newtonsoft.Json;
    using Avro;

    public class DecimalToAvroDecimalConverter : JsonConverter<AvroDecimal>
    {
        public override void WriteJson(JsonWriter writer, AvroDecimal value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override AvroDecimal ReadJson(JsonReader reader, Type objectType, AvroDecimal existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return default;

            try
            {
                decimal decimalValue = Convert.ToDecimal(reader.Value);

                return new AvroDecimal(decimalValue);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to convert {reader.Value} to Avro decimal.", ex);
            }
        }
    }
}
