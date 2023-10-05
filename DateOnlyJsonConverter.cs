using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NablaUtils
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly ReadJson(JsonReader reader,
            Type objectType,
            DateOnly existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) =>
            string.IsNullOrWhiteSpace(((string)reader.Value)) ? DateOnly.MinValue : DateOnly.ParseExact((string)reader.Value, Format, CultureInfo.InvariantCulture);

        public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer) =>
            writer.WriteValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}
