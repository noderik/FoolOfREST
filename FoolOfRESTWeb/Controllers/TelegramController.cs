using Microsoft.AspNetCore.Mvc;
using FoolOfRESTWeb.Models.ApiModels;

namespace FoolOfRESTWeb.Controllers;

public class TelegramController : Controller{


    public IActionResult Chats(){
        ViewData["ApiClient"] = new ApiClient();
        return View();
    }

    public IActionResult Users(){
        ViewData["ApiClient"] = new ApiClient();
        return View();
    }

    [Route("Telegram/Chat/{id:long}")]
    public IActionResult ChatData([FromRoute] long id){
        ApiClient api = new ApiClient();
        ChatApiModel? chat = api.GetChatAsync(id).GetAwaiter().GetResult();
        Console.WriteLine(chat);
        if (chat == null){
            return View("ChatNull");
        }
        return View("Chat", chat);
    }

    [Route("Telegram/User/{id:long}")]
    public IActionResult UserData([FromRoute] long id){
        ApiClient api = new ApiClient();
        UserApiModel? user = api.GetUserAsync(id).GetAwaiter().GetResult();
        Console.WriteLine(user);
        if (user == null){
            return View("UserNull");
        }
        return View("User", user);
    }
}

    public class ApiClient{

        public HttpClient _client;
        private string _apiKey;

        public ApiClient(){
            _client = new HttpClient();
            _client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5001/");
            _apiKey = ApiKey.Read();
            _client.DefaultRequestHeaders.Add("APIKEY", _apiKey);
        }

        public async Task<ChatApiModel?> GetChatAsync(long id){
            ChatApiModel? chat = null;
            HttpResponseMessage response = await _client.GetAsync($"chats/{id}");
            if (response.IsSuccessStatusCode){
                chat = await response.Content.ReadFromJsonAsync<ChatApiModel>();
            }

            if (chat == null) return chat;

            chat.Messages = await GetChatMessagesAsync(chat.Id);
            chat.Users = await GetChatUsersAsync(chat.Id);
            return chat;
        }

        public async Task<List<UserApiModel>?> GetChatUsersAsync(long chatId){
            List<UserApiModel>? users = null;
            HttpResponseMessage response = await _client.GetAsync($"chatusers/{chatId}");
            if (response.IsSuccessStatusCode){
                users = await response.Content.ReadFromJsonAsync<List<UserApiModel>>();
            }

            if (users == null) return users;
            return users;
        }

        public async Task<UserApiModel?> GetUserAsync(long id){
            UserApiModel? user = null;
            HttpResponseMessage response = await _client.GetAsync($"users/{id}");
            if (response.IsSuccessStatusCode){
                user = await response.Content.ReadFromJsonAsync<UserApiModel>();
            }

            if (user == null) return user;

            user.Messages = await GetUserMessagesAsync(user.Id);
            return user;
        }

        public async Task<List<UserApiModel>?> GetUsersAsync(){
            List<UserApiModel>? users = null; 
            HttpResponseMessage response = await _client.GetAsync("users/");
            Console.Write("Status code:");
            Console.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode){
                users = await response.Content.ReadFromJsonAsync<List<UserApiModel>>();
            }
            if (users != null){
                Console.WriteLine("Adding messages");
                foreach (UserApiModel user in users){
                    user.Messages = await GetUserMessagesAsync(user.Id);
                }
            }
            return users;
        }


        public async Task<List<ChatApiModel>?> GetChatsAsync(){
            List<ChatApiModel>? chats = null; 
            HttpResponseMessage response = await _client.GetAsync("chats/");
            Console.Write("Status code:");
            Console.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode){
                chats = await response.Content.ReadFromJsonAsync<List<ChatApiModel>>();
            }
            if (chats != null){
                Console.WriteLine("Adding messages");
                foreach (ChatApiModel chat in chats){
                    chat.Messages = await GetChatMessagesAsync(chat.Id);
                    chat.Users = await GetChatUsersAsync(chat.Id);
                }
            }
            return chats;
        }

        public async Task<List<MessageApiModel>?> GetChatMessagesAsync(long chatId){
            List<MessageApiModel>? messages = null;
            HttpResponseMessage response = await _client.GetAsync($"chatmessages/{chatId}");
            Console.WriteLine($"ChatMessages code: {response.StatusCode}");
            if (response.IsSuccessStatusCode){
                return await response.Content.ReadFromJsonAsync<List<MessageApiModel>>();
            }
            return messages;
        }

        public async Task<List<MessageApiModel>?> GetUserMessagesAsync(long userId){
            List<MessageApiModel>? messages = null;
            HttpResponseMessage response = await _client.GetAsync($"usermessages/{userId}");
            Console.WriteLine($"UserMessages code: {response.StatusCode}");
            if (response.IsSuccessStatusCode){
                return await response.Content.ReadFromJsonAsync<List<MessageApiModel>>();
            }
            return messages;
        }
    }
