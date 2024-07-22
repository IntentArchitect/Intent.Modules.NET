using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Players
{
    public class PlayerType
    {
        public PlayerType()
        {
        }

        public string? DeviceId { get; set; }
        public string? LanguageId { get; set; }
        public string? Ip { get; set; }
        public int SegmentId { get; set; }
        public string? Channel { get; set; }

        public static PlayerType Create(string? deviceId, string? languageId, string? ip, int segmentId, string? channel)
        {
            return new PlayerType
            {
                DeviceId = deviceId,
                LanguageId = languageId,
                Ip = ip,
                SegmentId = segmentId,
                Channel = channel
            };
        }
    }
}