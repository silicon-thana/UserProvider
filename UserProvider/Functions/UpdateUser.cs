using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Data.Contexts;
using Newtonsoft.Json;

namespace UserProvider.Functions
{
    public class UpdateUser
    {
        private readonly ILogger<UpdateUser> _logger;
        private readonly DataContext _context;

        public UpdateUser(ILogger<UpdateUser> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("UpdateUser")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateUser/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            ApplicationUser userToUpdate;
            try
            {
                userToUpdate = await _context.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Id == id);
                if (userToUpdate == null)
                {
                    return new NotFoundResult();
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var updatedUser = JsonConvert.DeserializeObject<ApplicationUser>(requestBody);

                // Update the user properties
                userToUpdate.FirstName = updatedUser.FirstName;
                userToUpdate.LastName = updatedUser.LastName;
                userToUpdate.Biography = updatedUser.Biography;
                userToUpdate.ProfileImg = updatedUser.ProfileImg;
                userToUpdate.IsSubscribed = updatedUser.IsSubscribed;
                userToUpdate.IsDarkTheme = updatedUser.IsDarkTheme;
                userToUpdate.NotificationEmail = updatedUser.NotificationEmail;

                // Update address if provided
                if (updatedUser.Address != null)
                {
                    userToUpdate.Address.AddressLine_1 = updatedUser.Address.AddressLine_1;
                    userToUpdate.Address.AddressLine_2 = updatedUser.Address.AddressLine_2;
                    userToUpdate.Address.PostalCode = updatedUser.Address.PostalCode;
                    userToUpdate.Address.City = updatedUser.Address.City;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the user: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkObjectResult(userToUpdate);
        }
    }
}
