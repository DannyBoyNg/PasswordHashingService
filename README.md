# PasswordHashingService

A simple service to hash passwords in C#.

## Dependancies

Microsoft.AspNetCore.Cryptography.KeyDerivation  
Microsoft.Extensions.Options  

## Installing

Install from Nuget
```
Install-Package DannyBoyNg.PasswordHashingService
```

## Usage

Console application

```csharp
using DannyBoyNg.Services;
...
var settings = new PasswordHashingSettings()
{
    IterationCount = 20000, //10000 is default
};
var passwordHashingService = new PasswordHashingService(settings);
var hash = passwordHashingService.HashPassword("mySuperSecretPass");
var result = passwordHashingService.VerifyHashedPassword(hash, "mySuperSecretPass");
switch (result)
{
    case PasswordVerificationResult.Success:
        // password is valid

        break;
    case PasswordVerificationResult.Failed:
        // password is invalid

        break;
    case PasswordVerificationResult.SuccessRehashNeeded:
        //password is valid but password needs to be rehashed because old hash might still be on a lower iteration count

        break;
}
```

ASP.NET Core  

Register service with dependency injection in Startup.cs
```csharp
using DannyBoyNg.Services;
...
public void ConfigureServices(IServiceCollection services)
{
    services.AddPasswordHashingService();

    or

    services.AddPasswordHashingService(options => {
        options.CacheSizeLimit = 20000; //Default: 10000
        options.CacheSlidingExpiration = TimeSpan.FromDays(1); //Default: TimeSpan.FromDays(3)
        options.UaStringSizeLimit = 256; //Default: 512
    });
}
```

## License

This project is licensed under the MIT License.

## Contributions

Contributions are welcome.
