using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Data.Contexts;

namespace UserProvider.Functions
{
    public class DeleteUser
    {
        private readonly ILogger<DeleteUser> _logger;
        private readonly DataContext _context;

        public DeleteUser(ILogger<DeleteUser> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("DeleteUser")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteUser/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to delete user with id {id}", id);

            ApplicationUser userToDelete;
            try
            {
                userToDelete = await _context.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Id == id);
                if (userToDelete == null)
                {
                    return new NotFoundResult();
                }

                _context.Users.Remove(userToDelete);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting the user: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkResult();
        }
    }
}