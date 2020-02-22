using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ServerlessFuncs
{
    public static class TodoApi
    {
        static List<Todo> todos = new List<Todo>();

        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating a new todo item.");
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);
            var todo = new Todo { TaskDescription = input.TaskDescription };
            todos.Add(todo);
            return new OkObjectResult(todo);
           
        }

        [FunctionName("GetTodos")]
        public static IActionResult GetTodos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("Getting todo list items.");
            return new OkObjectResult(todos);

        }

        [FunctionName("GetTodoById")]
        public static IActionResult GetTodoById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
           ILogger log, string id)
        {
            log.LogInformation("Getting a todo item.");

            var todo = todos.FirstOrDefault(t=>t.Id==id);
            if (todo != null) {
                return new OkObjectResult(todo);
            }
            return new NotFoundResult();

        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo(
           [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
           ILogger log, string id)
        {
                       
            var todo = todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return new NotFoundResult();
            }

            log.LogInformation("Updating a new todo item.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);

            todo.IsCompleted = input.IsCompleted;
            if (input.TaskDescription !=null)
               todo.TaskDescription = input.TaskDescription;
            
            return new OkObjectResult(todo);

        }

        [FunctionName("DeleteTodo")]
        public static IActionResult DeleteTodo(
         [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
         ILogger log, string id)
        {

            var todo = todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return new NotFoundResult();
            }

            log.LogInformation("Deleting a todo item.");
            todos.Remove(todo);
            return new OkResult();

        }
    }
}
