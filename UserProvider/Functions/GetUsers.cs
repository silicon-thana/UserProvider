using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Data.Contexts;

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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

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
                    u.IsSubscribed,
                    u.IsDarkTheme,
                    u.NotificationEmail,
                    Address = new
                    {
                        u.Address.Id,
                        u.Address.AddressLine_1,
                        u.Address.AddressLine_2,
                        u.Address.PostalCode,
                        u.Address.City
                    }
                })
                .ToListAsync();

            return new OkObjectResult(users);
        }
    }
}
