using System.Text.Json.Serialization;

namespace GPTWeb.GPTJsonClasses
{
	public class Message
	{
		[JsonPropertyName("role")]
		public string Role { get; set; } = "";
		[JsonPropertyName("content")]
		public string Content { get; set; } = "";
	}
}
