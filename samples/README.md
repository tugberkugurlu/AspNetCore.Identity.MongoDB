# ASP.NET Core Identity MongoDB Store Samples

This folder contains various samples to showcase how to use ASP.NET Core Identity with this MongoDB store provider.
There is not a huge difference to the standard use of ASP.NET Core Identity apart from hooking the MongoDB store provider. 
So, it's always helpful to go through [the ASP.NET Core Identity documentation first](https://docs.asp.net/en/latest/security/authentication/identity.html) if you haven't already.

## Samples

 * [IdentitySample.Mvc](./IdentitySample.Mvc): This is [the exact sample in Identity repository](https://github.com/aspnet/Identity/tree/1.0.0/samples/IdentitySample.Mvc), but it works with this MongoDB provider rather than the EntityFramework one. See [4c5fd98](https://github.com/tugberkugurlu/AspNetCore.Identity.MongoDB/commit/4c5fd986e4b4b6b63b1ffd7dd543c3f7a36b1b84) commit for the things that needed changing.