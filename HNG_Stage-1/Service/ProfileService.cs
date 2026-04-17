using HNG_Stage_1.Dto;
using HNG_Stage_1.External_API;
using HNG_Stage_1.Model;
using HNG_Stage_1.Repository;
using HNG_Stage_1.Utilities;
using Microsoft.EntityFrameworkCore;
using static HNG_Stage_1.External_API.IExternalApiService.ExternalApiService;

namespace HNG_Stage_1.Service
{
    public interface IProfileService
    {
        Task<(ProfileResponse profile, bool alreadyExists)> CreateProfileAsync(string name);
        Task<ProfileResponse?> GetProfileByIdAsync(Guid id);

        Task<List<ProfileListItemResponse>> GetAllProfilesAsync(string? gender, string? countryId, string? ageGroup);

        Task<bool> DeleteProfileAsync(Guid id);
    }
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _db;
        private readonly IExternalApiService _externalApiService;

        public ProfileService(AppDbContext db, IExternalApiService exter)
        {
            _db = db;
            _externalApiService = exter;
        }
        public async Task<(ProfileResponse profile, bool alreadyExists)> CreateProfileAsync(string name)
        {
            var existing = await _db.Profiles.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
            if (existing is not null)
                return (MapToResponse(existing), true);

            var genderTask = _externalApiService.GetGenderAsync(name);
            var ageTask = _externalApiService.GetAgeAsync(name);
            var natTask = _externalApiService.GetNationalityAsync(name);

            await Task.WhenAll(genderTask, ageTask, natTask);

            var genderData = await genderTask;
            var ageData = await ageTask;
            var natData = await natTask;

            if (string.IsNullOrWhiteSpace(genderData.Gender) || genderData.Count == 0)
                throw new ExternalApiException("Genderize");

            if (ageData.Age == 0)
                throw new ExternalApiException("Agify");

            if (natData.Country is null || natData.Country.Count == 0)
                throw new ExternalApiException("Nationalize");

            var topCountry = natData.Country.OrderByDescending(c => c.Probability).First();

            var profile = new Profile
            {
                Id = UuidV7.NewGuid(),
                Name = name.ToLower(),
                Gender = genderData.Gender,
                GenderProbability = genderData.Probability,
                SampleSize = genderData.Count,
                Age = ageData.Age,
                AgeGroup = ClassifyAge(ageData.Age),
                CountryId = topCountry.CountryId,
                CountryProbability = topCountry.Probability,
                CreatedAt = DateTime.UtcNow
            };

            _db.Profiles.Add(profile);
            await _db.SaveChangesAsync();

            return (MapToResponse(profile), false);
        }



        public async Task<bool> DeleteProfileAsync(Guid id)
        {
           var profile = await _db.Profiles.FindAsync(id);
              if (profile is null)
                return false;
              _db.Profiles.Remove(profile);
                await _db.SaveChangesAsync();
                return true;
        }

        public async Task<List<ProfileListItemResponse>> GetAllProfilesAsync(string? gender, string? countryId, string? ageGroup)
        {
            var query = _db.Profiles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(gender))
                query = query.Where(p => p.Gender!.ToLower() == gender.ToLower());

            if (!string.IsNullOrWhiteSpace(countryId))
                query = query.Where(p => p.CountryId.ToLower() == countryId.ToLower());

            if (!string.IsNullOrWhiteSpace(ageGroup))
                query = query.Where(p => p.AgeGroup.ToLower() == ageGroup.ToLower());

            return await query.OrderBy(p => p.CreatedAt)
                              .Select(p => new ProfileListItemResponse
                              {
                                  Id = p.Id.ToString(),
                                  Name = p.Name,
                                  Gender = p.Gender,
                                  Age = p.Age,
                                  AgeGroup = p.AgeGroup,
                                  CountryId = p.CountryId
                              }).ToListAsync();
        }

        public async Task<ProfileResponse?> GetProfileByIdAsync(Guid id)
        {
            var profile = await _db.Profiles.FindAsync(id);
            return profile is null ? null : MapToResponse(profile);
        }


        //--------------- helper ----------------
        private string ClassifyAge(int age) => age switch
        {
            <= 12 => "Child",
            >= 13 and <= 19 => "teenager",
            >= 20 and <= 59 => "adult",
            _ => "senior"
        };

        private ProfileResponse MapToResponse(Profile p) => new ProfileResponse
        {
            Id = p.Id.ToString(),
            Name = p.Name,
            Gender = p.Gender,
            GenderProbability = p.GenderProbability,
            SampleSize = p.SampleSize,
            Age = p.Age,
            AgeGroup = p.AgeGroup,
            CountryId = p.CountryId,
            CountryProbability = p.CountryProbability,
            CreatedAt = p.CreatedAt.ToUniversalTime()
                              .ToString("yyyy-MM-ddTHH:mm:ssZ")

        };


    }
}
