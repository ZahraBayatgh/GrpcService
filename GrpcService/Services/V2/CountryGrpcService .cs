using static Sample.gRPC.v2.CountryService;

namespace GrpcService.Services.V2;
public class CountryGrpcService : CountryServiceBase
{
    private readonly CountryManagementService _countryManagementService;
    public CountryGrpcService(CountryManagementService countryManagementService)
    {
        _countryManagementService = countryManagementService;
    }

}


