using Apress.Sample.gRPC;
using Google.Protobuf.WellKnownTypes;

namespace GrpcService.Services
{
    public class CountryManagementService
    {
        private readonly List<CountryReply> _countries = new(); // C# 9 syntax "Improved target typing"
        public CountryManagementService()
        {
            _countries.Add(new CountryReply 
            { 
                Id = 1,
                Name = "Canada",
                Description = "Canada has at least 32 000 lakes", 
                CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021, 1, 2), DateTimeKind.Utc))
            });


            _countries.Add(new CountryReply
            {
                Id = 2,
                Name = "USA",
                Description
            = "Yellowstone has 300 to 500 geysers",
                CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021, 1, 2), DateTimeKind.Utc))
            });
            _countries.Add(new CountryReply
            {
                Id = 3,
                Name = "Mexico",
                Description = "Mexico is crossed by Sierra Madre Oriental and Sierra Madre Occidental mountains",
                CreateDate = Timestamp.
            FromDateTime(DateTime.SpecifyKind(new DateTime(2021, 1, 2), DateTimeKind.Utc))
            });
        }

        public async Task<IEnumerable<CountryReply>> GetAllAsync()
        {
            return await Task.FromResult(_countries.ToArray());
        }

        public async Task<CountryReply> GetAsync(CountryIdRequest country)
        {
            return await Task.FromResult(_countries.FirstOrDefault(x => x.Id == country.Id));
        }

        public async Task DeleteAsync(IEnumerable<CountryIdRequest> countries)
        {
            var ids = countries.Select(x => x.Id).ToList();
            _countries.RemoveAll(x => ids.Contains(x.Id));
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(CountryUpdateRequest country)
        {
            var countryToUpdate = _countries.FirstOrDefault(x => x.Id == country.Id);
            if (countryToUpdate != null)
            {
                countryToUpdate.Description = country.Description; countryToUpdate.UpdateDate = country.UpdateDate;
            }


            await Task.CompletedTask;
        }

        public async Task<IEnumerable<CountryCreationReply>> CreateAsync(List<CountryCreationRequest> countries)
        {
            var countryCreationReply = new CountryCreationReply();
            var newCountries = new List<CountryReply>();
            var count = _countries.Count;
            countries.ForEach(country =>
            {
                var existingCountry = _countries.FirstOrDefault(x => x.Name == country.Name);
                if (existingCountry == null)
                {
                    newCountries.Add(new CountryReply
                    {
                        Id = ++count,
                        Name = country.Name,
                        Description = country.Description,
                        Flag = country.Flag,
                        CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021, 1, 2), DateTimeKind.Utc))
                    });
                }

            });

            _countries.AddRange(newCountries);
            return await Task.FromResult(newCountries.Select(x => new CountryCreationReply
            {
                Id = x.Id,
                Name = x.Name
            }).ToList());
        }
    }
}
