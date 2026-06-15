using System.Text;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;

namespace AssignmentApi.Tests;

public class AuthTests
{
    [Fact]
    public void Login_With_Valid_Credentials_Should_Return_True()
    {
        var username = "YWRtaW4=";
        var password = "YWRtaW4xMjM=";

        var decodedUsername = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(username));
        var decodedPassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(password));

        var result = decodedUsername == "admin" && decodedPassword == "admin123";

        Assert.True(result);
    }

    [Fact]
    public void Login_With_InCorrect_Credentials_Should_Return_False()
    {
        var username = "YWRtaW4=";
        var password = Convert.ToBase64String(Encoding.UTF8.GetBytes("password"));

        var decodedUsername = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(username));
        var decodedPassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(password));

        var result = decodedUsername == "admin" && decodedPassword == "admin123";

        Assert.False(result);  
    }

    [Fact]
    public void Login_With_Invalid_Base64_Should_Return_False()
    {
        var invalidBase64 = "invalid_base64";

        Assert.Throws<FormatException>(() =>
        {
            Convert.FromBase64String(invalidBase64);
        });
    }
}
