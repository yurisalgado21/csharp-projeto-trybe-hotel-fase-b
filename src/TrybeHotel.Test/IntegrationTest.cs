namespace TrybeHotel.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Diagnostics;
using System.Xml;
using System.IO;
using TrybeHotel.Dto;

public class ErrorJson {
    public string? message { get; set; }
}

public class LoginJson {
    public string? token { get; set; }
}


public class IntegrationTest: IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientTest;

    public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        //_factory = factory;
        _clientTest = factory.WithWebHostBuilder(builder => {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ContextTest>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDatabase");
                });
                services.AddScoped<ITrybeHotelContext, ContextTest>();
                services.AddScoped<ICityRepository, CityRepository>();
                services.AddScoped<IHotelRepository, HotelRepository>();
                services.AddScoped<IRoomRepository, RoomRepository>();
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ContextTest>())
                {
                    appContext.Database.EnsureCreated();
                    appContext.Database.EnsureDeleted();
                    appContext.Database.EnsureCreated();
                    appContext.Cities.Add(new City {CityId = 1, Name = "Manaus"});
                    appContext.Cities.Add(new City {CityId = 2, Name = "Palmas"});
                    appContext.SaveChanges();
                    appContext.Hotels.Add(new Hotel {HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1});
                    appContext.Hotels.Add(new Hotel {HotelId = 2, Name = "Trybe Hotel Palmas", Address = "Address 2", CityId = 2});
                    appContext.Hotels.Add(new Hotel {HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Addres 3", CityId = 1});
                    appContext.SaveChanges();
                    appContext.Rooms.Add(new Room { RoomId = 1, Name = "Room 1", Capacity = 2, Image = "Image 1", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 2, Name = "Room 2", Capacity = 3, Image = "Image 2", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 3, Name = "Room 3", Capacity = 4, Image = "Image 3", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 4, Name = "Room 4", Capacity = 2, Image = "Image 4", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 5, Name = "Room 5", Capacity = 3, Image = "Image 5", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 6, Name = "Room 6", Capacity = 4, Image = "Image 6", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 7, Name = "Room 7", Capacity = 2, Image = "Image 7", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 8, Name = "Room 8", Capacity = 3, Image = "Image 8", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 9, Name = "Room 9", Capacity = 4, Image = "Image 9", HotelId = 3 });
                    appContext.SaveChanges();
                    appContext.Users.Add(new User { UserId = 1, Name = "Ana", Email = "ana@trybehotel.com", Password = "Senha1", UserType = "admin" });
                    appContext.Users.Add(new User { UserId = 2, Name = "Beatriz", Email = "beatriz@trybehotel.com", Password = "Senha2", UserType = "client" });
                    appContext.Users.Add(new User { UserId = 3, Name = "Laura", Email = "laura@trybehotel.com", Password = "Senha3", UserType = "client" });
                    appContext.SaveChanges();
                    appContext.Bookings.Add(new Booking { BookingId = 1, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 2, RoomId = 1});
                    appContext.Bookings.Add(new Booking { BookingId = 2, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 3, RoomId = 4});
                    appContext.SaveChanges();
                }
            });
        }).CreateClient();
    }

    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Executando meus testes")]
    [InlineData("/city")]
    public async Task TestGet(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Testando a rota Post /city")]
    [InlineData("/city")]

    public async Task TestPostCity(string url)
    {
        var inputObj = new {
            Name = "Rio de Janeiro"
        };

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Testando a rota Post /city, se o city.Name é null")]
    [InlineData("/city")]

    public async Task TestPostCityException(string url)
    {
        var inputObj = new {
            Name = ""
        };

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }



    [Trait("Category", "Meus Testes")]
    [Theory(DisplayName = "Teste Rota /hotel")]
    [InlineData("/hotel")]

    public async Task TestGetHotel(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Trait("Category", "Meus Testes")]
    [Theory(DisplayName = "Teste Rota POST /hotel")]
    [InlineData("/hotel")]

    public async Task TestPostHotel(string url)
    {
        var inputObj = new {
            Name = "Trybe Hotel RJ",
            Address = "Avenida Atlântica, 1400",
            CityId = 2
        };

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Trait("Category", "Meus Testes")]
    [Theory(DisplayName = "Teste Rota POST /hotel EXCEPTION")]
    [InlineData("/hotel")]

    public async Task TestPostHotelException(string url)
    {
        var inputObj = new {
            Name = "",
            Address = "Avenida Atlântica, 1400",
            CityId = 2
        };

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Trait("Category", "Meus Testes")]
    [Theory(DisplayName = "Teste Rota /room/:hotelId")]
    [InlineData("/room/1")]

    public async Task TestGetRoomId(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Trait("Category", "Meus Testes")]
    [Theory(DisplayName = "Teste Rota /room/:hotelId EXCEPTION")]
    [InlineData("/room/ABC")]

    public async Task TestGetRoomIdEXCEPTION(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Trait("Category", "Meus Testes")]
    [Theory(DisplayName = "Teste Rota /room/:hotelId EXCEPTION Negative")]
    [InlineData("/room/-1")]

    public async Task TestGetRoomIdEXCEPTIONegative(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Trait("Category", "Meus Testes")]
    [Theory(DisplayName = "Teste Rota POST /room")]
    [InlineData("/room")]

    public async Task TestGPOSTRoom(string url)
    {
        var inputObj = new {
            Name = "Suite básica",
            Capacity = 2,
            Image = "image suite",
            HotelId = 1
        };

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Trait("Category", "Meus Testes")]
    [Theory(DisplayName = "Teste Rota DELETE /room/roomid")]
    [InlineData("/room/1")]

    public async Task TestGDELETERoomID(string url)
    {
        var response = await _clientTest.DeleteAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Trait("Category", "Meus Testes")]
    [Theory(DisplayName = "Teste Rota DELETE /room/roomid EXCEPTION")]
    [InlineData("/room/ABC")]

    public async Task TestGDELETERoomIDEXCEPTION(string url)
    {
        var response = await _clientTest.DeleteAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Trait("Category", "Meus Testes")]
    [Theory(DisplayName = "Teste Rota DELETE /room/roomid EXCEPTION NEGATIVE")]
    [InlineData("/room/-1")]

    public async Task TestGDELETERoomIDEXCEPTIONEGATIVE(string url)
    {
        var response = await _clientTest.DeleteAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    //REQUISITO 10.

    [Trait("Category", "Testando o endpoint POST /user")]
    [Theory(DisplayName = "Será validado que não é possível cadastrar um e-mail já cadastrado")]
    [InlineData("/user")]

    public async Task TestUserPostConflict(string url)
    {
        var inputObj = new {
            Name = "Ana", Email = "ana@trybehotel.com", Password = "Senha1",
        };

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
        ErrorJson jsonResponse = JsonConvert.DeserializeObject<ErrorJson>(responseString)!;
        Assert.Equal("User email already exists", jsonResponse!.message);
    }

    //
    [Trait("Category", "Testando o endpoint POST /user")]
    [Theory(DisplayName = "Será validado que é possível cadastrar um e-mail não cadastrado")]
    [InlineData("/user")]

    public async Task TestUserPostSuccess(string url)
    {
        var inputObj = new {
            Name = "Yuri", Email = "yuri@trybehotel.com", Password = "Senha123",
        };

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    //3.
    [Trait("Category", "Testando o endpoint POST /login")]
    [Theory(DisplayName = "Será validado que é possível logar um user cadastrado")]
    [InlineData("/login")]
    public async Task TestLoginPostSuccess(string url)
    {
        var inputObj = new {
            Name = "Ana", Email = "ana@trybehotel.com", Password = "Senha1",
        };

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    //
    [Trait("Category", "Testando o endpoint POST /login")]
    [Theory(DisplayName = "Será validado que não é possível logar sem um user cadastrado")]
    [InlineData("/login")]
    public async Task TestLoginPostFail(string url)
    {
        var inputObj = new {
            Name = "Ana", Email = "anaaaaaa@trybehotel.com", Password = "Senha1",
        };

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    //7.
    [Trait("Category", "Testando o endpoint POST /booking")]
    [Theory(DisplayName = "Será validado que é possível cadastrar uma nova reserva com sucesso")]
    [InlineData("/booking")]
    public async Task TestBookingPostSuccess(string url)
    {
        var inputLogin = new {
            Email = "ana@trybehotel.com", Password = "Senha1",
        };

        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(inputLogin), System.Text.Encoding.UTF8, "application/json"));

        var responseString = await responseLogin.Content.ReadAsStringAsync();
        LoginJson loginJson = JsonConvert.DeserializeObject<LoginJson>(responseString)!;

        var inputObj = new {
            CheckIn = "2030-08-27",
            CheckOut = "2030-08-28",
            GuestQuant = 1,
            RoomId = 1
        };

        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginJson.token);

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    //

    [Trait("Category", "Testando o endpoint POST /booking")]
    [Theory(DisplayName = "Caso o valor GuestQuant seja maior do que o valor de Capacity do quarto escolhido, a reserva não deve ser realizada.")]
    [InlineData("/booking")]
    public async Task TestBookingPostFail(string url)
    {
        var inputLogin = new {
            Email = "ana@trybehotel.com", Password = "Senha1",
        };

        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(inputLogin), System.Text.Encoding.UTF8, "application/json"));

        var responseString = await responseLogin.Content.ReadAsStringAsync();
        LoginJson loginJson = JsonConvert.DeserializeObject<LoginJson>(responseString)!;

        var inputObj = new {
            CheckIn = "2030-08-27",
            CheckOut = "2030-08-28",
            GuestQuant = 100,
            RoomId = 1
        };

        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginJson.token);

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    //

    [Trait("Category", "Testando o endpoint GET /booking")]
    [Theory(DisplayName = "Será validado que é possível filtrar uma reserva com sucesso")]
    [InlineData("/booking/1")]
    public async Task TestBookingGet(string url)
    {
        var inputLogin = new {
            Email = "ana@trybehotel.com", Password = "Senha1",
        };

        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(inputLogin), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await responseLogin.Content.ReadAsStringAsync();
        LoginJson loginJson = JsonConvert.DeserializeObject<LoginJson>(responseString)!;

        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginJson.token);

        var response = await _clientTest.GetAsync(url);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    //
    [Trait("Category", "Testando o endpoint GET /booking")]
    [Theory(DisplayName = "Será validado que não é possível filtrar uma reserva")]
    [InlineData("/booking/1")]
    public async Task TestBookingGetfail(string url)
    {
        var inputLogin = new {
            Email = "anaaaaaa@trybehotel.com", Password = "Senha111",
        };

        var responseLogin = await _clientTest.PostAsync("/login", new StringContent(JsonConvert.SerializeObject(inputLogin), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await responseLogin.Content.ReadAsStringAsync();
        LoginJson loginJson = JsonConvert.DeserializeObject<LoginJson>(responseString)!;

        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginJson.token);

        var response = await _clientTest.GetAsync(url);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    //8.
    [Trait("Category", "Testando o endpoint GET /user")]
    [Theory(DisplayName = "Será validado que é possível filtrar uma user")]
    [InlineData("/user")]
    public async Task TestuserGetSuccess(string url)
    {
        var inputLogin = new {
            Email = "ana@trybehotel.com",
            Password = "Senha1"
        };
        var responseLogin = await _clientTest.PostAsync("/login",new StringContent(JsonConvert.SerializeObject(inputLogin), System.Text.Encoding.UTF8, "application/json"));
        var responseLoginString = await responseLogin.Content.ReadAsStringAsync();
        LoginJson jsonLogin = JsonConvert.DeserializeObject<LoginJson>(responseLoginString)!;

        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jsonLogin.token);

        var response = await _clientTest.GetAsync(url);
        var responseString = await response.Content.ReadAsStringAsync();
        List<UserDto> jsonResponse = JsonConvert.DeserializeObject<List<UserDto>>(responseString)!;
        jsonResponse = jsonResponse.OrderBy(u => u.userId).ToList();

        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
        Assert.Contains("ana@trybehotel.com", jsonResponse[0].Email);
        Assert.Contains("beatriz@trybehotel.com", jsonResponse[1].Email);
        Assert.Contains("laura@trybehotel.com", jsonResponse[2].Email);

        Assert.Contains("admin", jsonResponse[0].userType);
        Assert.Contains("client", jsonResponse[1].userType);
        Assert.Contains("client", jsonResponse[2].userType);
    }
}