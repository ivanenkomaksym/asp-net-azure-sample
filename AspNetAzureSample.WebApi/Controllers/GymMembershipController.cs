using AspNetAzureSample.Security;
using AspNetAzureSample.UserProviders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetAzureSample.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.MinimumAgePolicy)]
    [Route("api/[controller]")]
    public class GymMembershipController : ControllerBase
    {
        public GymMembershipController(IUserProvider userProvider)
        {
            UserProvider = userProvider;
        }

        /// <summary>
        /// Retrieves details about the current user's gym membership.
        /// </summary>
        /// <returns>Membership details.</returns>
        [HttpGet("details")]
        public async Task<IActionResult> GetMembershipDetails()
        {
            var userId = UserProvider.GetUserName(HttpContext);

            var membershipInfo = new
            {
                UserId = userId,
                MembershipType = "Premium",
                StartDate = "2023-01-01",
                EndDate = "2024-12-31",
                AccessLevel = "Full Access"
            };

            return Ok(membershipInfo);
        }

        /// <summary>
        /// Allows the current user to enroll in a specific class.
        /// </summary>
        /// <param name="className">The name of the class to enroll in.</param>
        /// <returns>A confirmation message.</returns>
        [HttpPost("enroll/{className}")]
        public async Task<IActionResult> EnrollInClass(string className)
        {
            var userId = UserProvider.GetUserName(HttpContext);

            return Ok($"User '{userId}' successfully enrolled in '{className}'.");
        }

        /// <summary>
        /// Retrieves a list of available gym classes.
        /// </summary>
        /// <returns>A list of class names.</returns>
        [HttpGet("classes")]
        public async Task<IActionResult> GetAvailableClasses()
        {
            var classes = new List<string>
            {
                "Yoga (Beginner)",
                "Spin Class",
                "HIIT Training",
                "Zumba",
                "Weightlifting Fundamentals"
            };
            return Ok(classes);
        }

        private readonly IUserProvider UserProvider;
    }
}