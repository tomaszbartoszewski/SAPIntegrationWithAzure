using System.IO;

namespace OutboundStudent
{
    public class FormatCustomHelper
    {
        public string Key => "formatCustom";
        public void Write(TextWriter writer, object context, object[] parameters)
        {
            if (parameters.Length < 1)
                return;

            var property = parameters[0];
            if (parameters.Length < 2)
            {
                writer.Write(property);
                return;
            }
            var format = $"{{0:{parameters[1]}}}";
            writer.Write(format, property);
        }
    }
}
