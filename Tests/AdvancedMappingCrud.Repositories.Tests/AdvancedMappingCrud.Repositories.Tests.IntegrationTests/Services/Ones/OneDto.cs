using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Ones
{
    public class OneDto
    {
        public OneDto()
        {
            OneName1 = null!;
            Fives = null!;
            FourName4 = null!;
            ThreeName3 = null!;
            TwoName2 = null!;
        }

        public Guid Id { get; set; }
        public string OneName1 { get; set; }
        public List<OneFiveDto> Fives { get; set; }
        public string FourName4 { get; set; }
        public string ThreeName3 { get; set; }
        public string TwoName2 { get; set; }
        public int OneAge1 { get; set; }
        public int FourAge4 { get; set; }
        public int ThreeAge3 { get; set; }
        public int TwoAge2 { get; set; }

        public static OneDto Create(
            Guid id,
            string oneName1,
            List<OneFiveDto> fives,
            string fourName4,
            string threeName3,
            string twoName2,
            int oneAge1,
            int fourAge4,
            int threeAge3,
            int twoAge2)
        {
            return new OneDto
            {
                Id = id,
                OneName1 = oneName1,
                Fives = fives,
                FourName4 = fourName4,
                ThreeName3 = threeName3,
                TwoName2 = twoName2,
                OneAge1 = oneAge1,
                FourAge4 = fourAge4,
                ThreeAge3 = threeAge3,
                TwoAge2 = twoAge2
            };
        }
    }
}