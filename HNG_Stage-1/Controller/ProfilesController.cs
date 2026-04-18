using HNG_Stage_1.Dto;
using HNG_Stage_1.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using static HNG_Stage_1.External_API.ExternalApiService;


namespace HNG_Stage_1.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest? request)
        {

           
            if (request is null)
                return UnprocessableEntity( new
                {
                    status = 422,
                    message = "Request body is invalid or missin"

                });

            if (!Regex.IsMatch(request.Name!, @"^[a-zA-Z\s]+$"))
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "Profile name must contain only letters."
                });
            }

            
            try
            {
                var (profile, alreadyExists) = await _profileService.CreateProfileAsync(request.Name.Trim());
                if (alreadyExists)
                    return Ok(new
                    {
                        status = "success",
                        message = "Profile already exists.",
                        data = profile
                    });

                return StatusCode(201, new {status= "success", data = profile});
            }
            catch (ExternalApiException ex)
            {
                return StatusCode(502, Error(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred: "));
            }

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfileById(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return NotFound(new
                {
                    status = 404,
                    message = "Profile not found."
                });
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(guid);
                if(profile is null)
                    return NotFound(new
                    {
                        status = 404,
                        message = "Profile not found."
                    });
                return Ok(new {status = "success",
                    data = profile
                });
            }catch(Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred."));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProfile([FromQuery] string? gender, [FromQuery] string? country_Id, [FromQuery] string? age_Group)
        {
            try
            {
                var profiles = await _profileService.GetAllProfilesAsync(gender, country_Id, age_Group);
                return Ok(new
                {
                    status = "success",
                    count = profiles.Count,
                    data = profiles
                });
            }

            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred."));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return NotFound(new
                {
                    status = 404,
                    message = "Profile not found."
                });
            try
            {
                var deleted = await _profileService.DeleteProfileAsync(guid);
                if (!deleted)
                    return NotFound(new
                    {
                        status = 404,
                        message = "Profile not found."
                    });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred."));
            }
        }

        //------ Helper ---------
        private static object Error(string message) => new { status = "error", message };
    }
}

