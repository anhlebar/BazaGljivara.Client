//LocationService i AccountService  koriste Http service za komunikaciju sa serverom
//Ovo je lažni backend koji preuzima pathove http zahtjeva i routa ih na odredjene metode


using BazaGljivara.Client.Models;
using BazaGljivara.Client.Models.Accounts;
using BazaGljivara.Client.Models.Locations;
using BazaGljivara.Client.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BazaGljivara.Client.Helpers
{
    public class FakeBackendHandler : HttpClientHandler
    {
        private ILocalStorageService _localStorageService;

        public FakeBackendHandler(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // ime arraya u local storageu u kojem drzimo korisnike
            // ( f12 u browseru, application/local storage za debugiranje vrijednosti )
            var usersKey = "blazor-users";
            var users = await _localStorageService.GetItem<List<UserRecord>>(usersKey) ?? new List<UserRecord>();

            // ime arraya u local storageu u kojem drzimo lokacije Smrcaka
            var locationsKey = "blazor-mushroom-locations";
            var locs = await _localStorageService.GetItem<List<LocationRecord>>(locationsKey) ?? new List<LocationRecord>();


            var method = request.Method;
            var path = request.RequestUri.AbsolutePath;

            return await handleRoute();

            async Task<HttpResponseMessage> handleRoute()
            {    
                //rute za korisnicki CRUD
                if (path == "/users/authenticate" && method == HttpMethod.Post)
                    return await authenticate();
                if (path == "/users/register" && method == HttpMethod.Post)
                    return await register();
                if (path == "/users" && method == HttpMethod.Get)
                    return await getUsers();
                if (Regex.Match(path, @"\/users\/\d+$").Success && method == HttpMethod.Get)
                    return await getUserById();
                if (Regex.Match(path, @"\/users\/\d+$").Success && method == HttpMethod.Put)
                    return await updateUser();
                if (Regex.Match(path, @"\/users\/\d+$").Success && method == HttpMethod.Delete)
                    return await deleteUser();

                //route za gljive

                //sve lokacije
                if (path == "/locations")
                    return await locations();
                //stvori novu, a.k.a. register
                if (path == "/locations/register" )
                    return await registerLocation();
                //detalji lokacije (lat, lng)
                if (Regex.Match(path, @"\/locations\/\d+$").Success && method == HttpMethod.Get)
                    return await getLocationById();
                //obriši
                if (Regex.Match(path, @"\/locations\/\d+$").Success && method == HttpMethod.Delete)
                    return await deleteLocation();
                //mjenjanje lokacije
                if (Regex.Match(path, @"\/locations\/\d+$").Success && method == HttpMethod.Put)
                    return await updateLocation();



                // pass through za zahtjeve koji ne zadovoljavaju pravila, vodi na glavnu , bez greske
                return await base.SendAsync(request, cancellationToken);
            }

            async Task<HttpResponseMessage> authenticate()
            {
                var bodyJson = await request.Content.ReadAsStringAsync();
                var body = JsonSerializer.Deserialize<BazaGljivara.Client.Models.Accounts.Login>(bodyJson);
                var user = users.FirstOrDefault(x => x.Username == body.Username && x.Password == body.Password);

                if (user == null)
                    return await error("Korisničko ime ili lozinka ne odgovaraju niti jednom korisniku");

                return await ok(new
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = "fake-jwt-token"
                });
            }


            /*
             * metode ruta za lokacije gljiva
             * 
             * Ove metode sam dodao sam , u biti su iste kao i metode za korisnike koje su bile
             * razrađene u tutorialu
             * 
             */
            

            //vraca lokacije, getAll metoda u Pages/Sightings/index.razor ju koristi
            async Task<HttpResponseMessage> locations()
            {
                if (!isLoggedIn()) return await unauthorized();
                return await ok(locs.Select(x => locDetails(x)));
                //return await ok();

            }


            //dohvaca podatke jedne lokacije
            async Task<HttpResponseMessage> getLocationById()
            {
                if (!isLoggedIn()) return await unauthorized();

                var loc = locs.FirstOrDefault(x => x.Id == idFromPath());
                return await ok(locDetails(loc));
            }


            //stvara novu lokaciju
            async Task<HttpResponseMessage> registerLocation()
            {

               //preuzmi serijalizirane podatke iz tijela zahtjeva
               var bodyJson = await request.Content.ReadAsStringAsync();
               var body = JsonSerializer.Deserialize<AddLocation>(bodyJson);

               var l = new LocationRecord
                {
                    Id = locs.Count > 0 ? locs.Max(x => x.Id) + 1 : 1,
                    Lat = body.Lat,
                    Lng = body.Lng,
         
                };

                locs.Add(l);
                //sacuvaj u local storage browsera
                await _localStorageService.SetItem(locationsKey, locs);
                return await ok();
            }

            //mjenja podatke
            async Task<HttpResponseMessage> updateLocation()
            {
                if (!isLoggedIn()) return await unauthorized();

                var bodyJson = await request.Content.ReadAsStringAsync();
                var body = JsonSerializer.Deserialize<EditLocation>(bodyJson);
                var location = locs.FirstOrDefault(x => x.Id == idFromPath());

                // update and save user
                location.Lat = body.Lat;
                location.Lng = body.Lng;
                await _localStorageService.SetItem(locationsKey, locs);
            
                return await ok();
            }

            async Task<HttpResponseMessage> deleteLocation()
            {
                if (!isLoggedIn()) return await unauthorized();

                locs.RemoveAll(x => x.Id == idFromPath());
                await _localStorageService.SetItem(locationsKey, locs);

                return await ok();
            }


            //korisničke metode
            async Task<HttpResponseMessage> register()
            {
                var bodyJson = await request.Content.ReadAsStringAsync();
                var body = JsonSerializer.Deserialize<AddUser>(bodyJson);

                if (users.Any(x => x.Username == body.Username))
                    return await error($"Korisničko ime  '{body.Username}' je zauzeto");

                var user = new UserRecord
                {
                    Id = users.Count > 0 ? users.Max(x => x.Id) + 1 : 1,
                    Username = body.Username,
                    Password = body.Password,
                    FirstName = body.FirstName,
                    LastName = body.LastName
                };

                users.Add(user);

                await _localStorageService.SetItem(usersKey, users);

                return await ok();
            }

            async Task<HttpResponseMessage> getUsers()
            {
                if (!isLoggedIn()) return await unauthorized();
                return await ok(users.Select(x => basicDetails(x)));
            }

            async Task<HttpResponseMessage> getUserById()
            {
                if (!isLoggedIn()) return await unauthorized();

                var user = users.FirstOrDefault(x => x.Id == idFromPath());
                return await ok(basicDetails(user));
            }

            async Task<HttpResponseMessage> updateUser()
            {
                if (!isLoggedIn()) return await unauthorized();

                var bodyJson = await request.Content.ReadAsStringAsync();
                var body = JsonSerializer.Deserialize<BazaGljivara.Client.Models.Accounts.EditUser>(bodyJson);
                var user = users.FirstOrDefault(x => x.Id == idFromPath());

                // if username changed check it isn't already taken
                if (user.Username != body.Username && users.Any(x => x.Username == body.Username))
                    return await error($"Korisničko ime '{body.Username}' je zauzeto");

                // only update password if entered
                if (!string.IsNullOrWhiteSpace(body.Password))
                    user.Password = body.Password;

                // update and save user
                user.Username = body.Username;
                user.FirstName = body.FirstName;
                user.LastName = body.LastName;
                await _localStorageService.SetItem(usersKey, users);

                return await ok();
            }

            async Task<HttpResponseMessage> deleteUser()
            {
                if (!isLoggedIn()) return await unauthorized();

                users.RemoveAll(x => x.Id == idFromPath());
                await _localStorageService.SetItem(usersKey, users);

                return await ok();
            }

            // helper functions

            async Task<HttpResponseMessage> ok(object body = null)
            {
                return await jsonResponse(HttpStatusCode.OK, body ?? new { });
            }

            async Task<HttpResponseMessage> error(string message)
            {
                return await jsonResponse(HttpStatusCode.BadRequest, new { message });
            }

            async Task<HttpResponseMessage> unauthorized()
            {
                return await jsonResponse(HttpStatusCode.Unauthorized, new { message = "Unauthorized" });
            }

            async Task<HttpResponseMessage> jsonResponse(HttpStatusCode statusCode, object content)
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
                };

                // delay to simulate real api call
                await Task.Delay(500);

                return response;
            }

            bool isLoggedIn()
            {
                return request.Headers.Authorization?.Parameter == "fake-jwt-token";
            }

            int idFromPath()
            {
                return int.Parse(path.Split('/').Last());
            }

            dynamic basicDetails(UserRecord user)
            {
                return new
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
            }

            //konvertira locationrecord
            dynamic locDetails(LocationRecord loc)
            {
                return new
                {
                    Id = loc.Id.ToString(),
                    Lat = loc.Lat,
                    Lng = loc.Lng,
                  
                };
            }

        }
    }

    // klase za recorde koje sprema lažni backend

    public class UserRecord
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LocationRecord
    {
        public int Id { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
      
    }


}
