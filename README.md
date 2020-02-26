# StuV.Http
StuV.Http is a local http server that acts as stub for unit test.  
StuV.Http is build on [ASP.NET Core Web Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-2.2) via .NET Standard 2.0.

NuGet: [StuV.Http](https://www.nuget.org/packages/StuV.Http/)
```
Install-Package StuV.Http
```

## Usage
Adding patterns of http request and their response by using When() and Return() method.
```C#
// create and setup StubHttpServer
var stubHttp = new StubHttpServer(5555);

stubHttp.When(req => req.RequestUri.PathAndQuery == "/users/u001")
        .Return(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("response body of u001") });

stubHttp.When(req => req.RequestUri.PathAndQuery == "/users/u002")
        .Return(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("response body of u002") });

stubHttp.When(req => req.Method == HttpMethod.Post
                  && req.RequestUri.PathAndQuery == "/users"
                  && req.Content.ReadAsStringAsync().Result.Contains("John Doe"))
        .Return(() => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StreamContent(File.OpenRead("SampleData_JohnDoe.json"))
        });


// by way of example
var client = new HttpClient();
var actual = await client.GetStringAsync("http://localhost:5555/users/u001");
Assert.Equal("response body of u001", actual);
```

### Also have helpful extension methods.
```C#
stubHttp.WhenGet("/users/u001")
        .ReturnJson(new { Id = "u001", Name = "Jane Doe" });

stubHttp.WhenPost("/users", body => body.Contains("John Doe"))
        .ReturnFile("SampleData_JohnDoe.json");
```
