using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Functions.Worker;

namespace UserProvider.Functions
{
    public class GetUserById
    {
        private readonly ILogger<GetUserById> _logger;
        private readonly DataContext _context;

        public GetUserById(ILogger<GetUserById> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetUserById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetUser/{id}")] HttpRequest req,
            string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to get user by ID.");

            try
            {
                var user = await _context.Users
                                         .Include(u => u.Address)
                                         .Where(u => u.Id == id)
                                         .Select(u => new
                                         {
                                             u.Id,
                                             u.UserName,
                                             u.Email,
                                             u.FirstName,
                                             u.LastName,
                                             u.Biography,
                                             u.ProfileImg,
                                             u.AddressId,
                                             Address = u.Address == null ? null : new
                                             {
                                                 u.Address.Id,
                                                 u.Address.AddressLine_1,
                                                 u.Address.AddressLine_2,
                                                 u.Address.PostalCode,
                                                 u.Address.City
                                             },
                                             u.IsSubscribed,
                                             u.IsDarkTheme,
                                             u.NotificationEmail
                                         })
                                         .FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found.");
                    return new NotFoundObjectResult(new { Status = 404, Message = "User not found." });
                }

                return new OkObjectResult(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user with ID {id}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
