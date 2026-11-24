using System.Net;
using eShop.Catalog.API.Services;

namespace eShop.Catalog.FunctionalTests;

public class SafeOutboundHttpTests
{
    [Theory]
    [InlineData("169.254.169.254")]
    [InlineData("127.0.0.1")]
    [InlineData("10.0.0.5")]
    [InlineData("172.16.0.1")]
    [InlineData("192.168.1.1")]
    [InlineData("0.0.0.0")]
    [InlineData("::1")]
    [InlineData("fe80::1")]
    [InlineData("fd00::1")]
    [InlineData("::ffff:169.254.169.254")]
    public void RejectsNonPublicAddresses(string ip)
        => Assert.False(SafeOutboundHttp.IsPublicAddress(IPAddress.Parse(ip)));

    [Theory]
    [InlineData("8.8.8.8")]
    [InlineData("2606:4700:4700::1111")]
    public void AllowsPublicAddresses(string ip)
        => Assert.True(SafeOutboundHttp.IsPublicAddress(IPAddress.Parse(ip)));

    [Theory]
    [InlineData("file:///etc/passwd", false)]
    [InlineData("ftp://example.com/x", false)]
    [InlineData("not a url", false)]
    [InlineData("https://user:pw@example.com/", false)]
    [InlineData("https://example.com/feed.json", true)]
    public void ValidatesScheme(string url, bool expected)
        => Assert.Equal(expected, SafeOutboundHttp.IsAllowedUrl(url));

    [Fact]
    public async Task ClientRefusesMetadataAddress()
    {
        using var http = SafeOutboundHttp.CreateClient();
        await Assert.ThrowsAsync<HttpRequestException>(() => http.GetStringAsync("http://169.254.169.254/latest/meta-data/"));
    }
}
