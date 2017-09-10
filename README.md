# AspNetCore.Identity.MongoDB [![Build Status](https://travis-ci.org/tugberkugurlu/AspNetCore.Identity.MongoDB.svg?branch=master)](https://travis-ci.org/tugberkugurlu/AspNetCore.Identity.MongoDB)

[MongoDB](https://www.mongodb.com/) data store adaptor for [ASP.NET Core Identity](https://github.com/aspnet/Identity), which allows you to build ASP.NET Core web applications, including membership, login, and user data. With this library, you can store your user's membership related data on MongoDB.

## Using the Library

[The library is available at NuGet.org](https://www.nuget.org/packages/AspNetCore.Identity.MongoDB). This library supports [`netstandard1.6`](https://docs.microsoft.com/en-us/dotnet/articles/standard/library).

### Samples

You can find some samples under [./samples](./samples) folder and each of the sample contain a README file on its own with the instructions showing how to run them.

### Tests

In order to be able to run the tests, you need to have MongoDB up and running on `localhost:27017`. You can easily do this by running the below Docker command:

```bash
docker run --name some-mongo -d -p "27017:27017" mongo:3
```

After that, you can run the tests through your prefered test runner (e.g. JetBrains Rider test runner) or by invoking the `dotnet test` command under the test project directory.

## Contributors

 - [Matt Whetton](https://github.com/mattwhetton)
 - [Soren Zand](https://github.com/SorenZ)
 - [Rethabile Mokoena](https://github.com/rm2k)
 - [Vladimir Kucher](https://github.com/vladimir-kucher)

## License

The MIT License (MIT)

Copyright (c) 2016 Tugberk Ugurlu

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
