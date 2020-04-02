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
    Prf = KeyDerivationPrf.HMACSHA256, //default: KeyDerivationPrf.HMACSHA256
    Pbkdf2IterCount = 20000, //10000 is default
    Pbkdf2SaltSize = 16, //default: 16 (bytes)
    Pbkdf2NumBytesRequested = 32, //default: 32 (bytes)
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
    //You don't have to provide settings if you want to use the defaults
    services.AddPasswordHashingService(options => {
        options.Prf = KeyDerivationPrf.HMACSHA256; //default: KeyDerivationPrf.HMACSHA256
        options.Pbkdf2IterCount = 10000; //default: 10000 (iterations)
        options.Pbkdf2SaltSize = 16; //default: 16 (bytes)
        options.Pbkdf2NumBytesRequested = 32; //default: 32 (bytes)
    });}
```

Inject IPasswordHashingService into a controller or wherever.
```csharp
public class MyController: ControllerBase
{
    private readonly IPasswordHashingService passwordHashingService;

    public MyController(IPasswordHashingService passwordHashingService) // <-- Inject IPasswordHashingService here
    {
        this.passwordHashingService = passwordHashingService;
    }

    //Example usage
    public ActionResult MyAction()
    {
        //Create a hash from a password
        var hash = passwordHashingService.HashPassword("mySuperSecretPass");

        //Validate hash with a password
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
                //password is valid but password needs to be rehashed because old hash might still be on a lower Pbkdf2 iteration count
                //rehash password here and commit to the database

                //goto the Success case
                goto case PasswordVerificationResult.Success;
        }
        return Ok();
    }
}
```

## License

This project is licensed under the MIT License.

## Contributions

Contributions are welcome.
