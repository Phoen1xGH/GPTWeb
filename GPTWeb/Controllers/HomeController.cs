using GPTWeb.Models;
using GPTWeb.GPTJsonClasses;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace GPTWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly StringBuilder _gptMessage = new();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
       
		public IActionResult Index()
        {
	        return View();
        }

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
		public ActionResult ChatGPTResponse(string userMessage) 
        {
	        _gptMessage.Append($"User: {userMessage}\nChatGPT: ");
	        string apiKey = "your-secret-key";
	        // адрес api для взаимодействия с чат-ботом
	        string endpoint = "https://api.openai.com/v1/chat/completions";
	        // набор соообщений диалога с чат-ботом
	        List<Message> messages = new List<Message>();
	        // HttpClient для отправки сообщений
	        var httpClient = new HttpClient();

	        var message = new Message() { Role = "user", Content = userMessage };
	        // добавляем сообщение в список сообщений
	        messages.Add(message);

	        // формируем отправляемые данные
	        var requestData = new Request()
	        {
		        ModelId = "gpt-3.5-turbo",
		        Messages = messages
	        };

	        // устанавливаем отправляемый в запросе токен
	        httpClient.DefaultRequestHeaders.Clear();
	        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
			// отправляем запрос
			
	        using var response =  httpClient.PostAsJsonAsync(endpoint, requestData).Result;

	        if (!response.IsSuccessStatusCode)
	        {
		        ErrorMessage($"{(int)response.StatusCode} {response.StatusCode}");
		        return View("Index");
	        }
			ResponseData? responseData = response.Content.ReadFromJsonAsync<ResponseData>().Result;

			var choices = responseData?.Choices ?? new List<Choice>();
			if (choices.Count == 0)
			{
				ErrorMessage("No choices were returned by the API :(");
				return View("Index");
			}
			var choice = choices[0];
			var responseMessage = choice.Message;
			// добавляем полученное сообщение в список сообщений
			messages.Add(responseMessage);
			_gptMessage.Append(responseMessage.Content.Trim());
            ViewBag.Message = _gptMessage;
			return View("Index");
        }

		private void ErrorMessage(string error)
		{
			_gptMessage.Clear();
			_gptMessage.Append(error);
			ViewBag.Message = _gptMessage;
		}
    }
}
