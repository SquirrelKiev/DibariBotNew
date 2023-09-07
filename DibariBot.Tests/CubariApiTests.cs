//using Newtonsoft.Json;
//using NSubstitute;
//using RichardSzalay.MockHttp;
//using System.Net;

//namespace DibariBot.Tests;

//public class CubariApiTests
//{
//    public static IHttpClientFactory GetMockedFactory(string response, HttpStatusCode code)
//    {
//        const string BASE_ADDRESS = "https://example.com";

//        var handler = new MockHttpMessageHandler();
//        handler.When($"{BASE_ADDRESS}/read/api/testplatform/series/testseries").Respond("application/json", );

//        var client = handler.ToHttpClient();
//        client.BaseAddress = new Uri(BASE_ADDRESS);

//        var httpClientFactory = Substitute.For<IHttpClientFactory>();
//        httpClientFactory.CreateClient(CubariApi.CUBARI_CLIENT_NAME).Returns(client);

//        return httpClientFactory;
//    }

//    [Fact]
//    public async Task GetManga_ReturnsExpectedManga()
//    {
//        var chapterSchema = new CubariMangaSchema()
//        {
//            title = "testTitle",
//        };

//        var factory = GetMockedFactory(JsonConvert.SerializeObject(chapterSchema), HttpStatusCode.OK);

//        var api = new CubariApi(factory);
//        var identifier = new SeriesIdentifier { platform = "testPlatform", series = "testSeries" };
//        var manga = await api.GetManga(identifier);

//        Assert.Equal("testTitle", manga.Title);
//    }
//}
