//service za lokacije smrcaka, isti kao i za korisnicke racune
//koristene route su definirane u Helpers/FakeBackendHandler
using BazaGljivara.Client.Models;
using BazaGljivara.Client.Models.Locations;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BazaGljivara.Client.Services
{
    public interface ILocationsService
    {
        Location Location { get; }
        Task Initialize();
        Task Register(AddLocation model);
        Task<IList<Location>> GetAll();
        Task<Location> GetById(string id);
        Task Update(string id, EditLocation model);
        Task Delete(string id);
    }

    public class LocationsService : ILocationsService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private string _userKey = "user";

        public Location Location { get; private set; }

        public LocationsService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }



        public async Task Initialize()
        {
          
        }

        //registrira novu lokaciju
        public async Task Register(AddLocation model)
        {
            await _httpService.Post("/locations/register", model);
        }

        //lista sve lokacije
        public async Task<IList<Location>> GetAll()
        {
            return await _httpService.Get<IList<Location>>("/locations");
        }

        //
        public async Task<Location> GetById(string id)
        {
            return await _httpService.Get<Location>($"/locations/{id}");
        }

        //
        public async Task Update(string id, EditLocation model)
        {
            await _httpService.Put($"/locations/{id}", model);

        }

        public async Task Delete(string id)
        {
            await _httpService.Delete($"/locations/{id}");
           
        }
    }
}
