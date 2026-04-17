namespace HNG_Stage_1.Model
{
    public class Profile
    {
        public Guid Id { get; set; } // UUID v7 (we’ll handle this)

        public string Name { get; set; } =string.Empty;

        public string? Gender { get; set; } 
        public double GenderProbability { get; set; }
        public int SampleSize { get; set; }

        public int Age { get; set; }
        public string AgeGroup { get; set; } = string.Empty;

        public string CountryId { get; set; } = string.Empty;
        public double CountryProbability { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
