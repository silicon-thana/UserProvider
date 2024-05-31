using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Functions.Worker;

namespace UserProvider.Functions
{
    public class GetUsers
    {
        private readonly ILogger<GetUsers> _logger;
        private readonly DataContext _context;

        public GetUsers(ILogger<GetUsers> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetUsers")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to get all users.");

            try
            {
                var users = await _context.Users
                                          .Include(u => u.Address)
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
                                              u.Address,
                                              u.IsSubscribed,
                                              u.IsDarkTheme,
                                              u.NotificationEmail
                                          })
                                          .ToListAsync();

                return new OkObjectResult(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving users: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
