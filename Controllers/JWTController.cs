using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace woodgrove_groceries_api.Controllers;

[ApiController]
[Route("[controller]")]
public class JWTController : ControllerBase
{
    private readonly ILogger<JWTController> _logger;
    private readonly IConfiguration _configuration;
    readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;

    public JWTController(ILogger<JWTController> logger, IConfiguration configuration, IAuthorizationHeaderProvider authorizationHeaderProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _authorizationHeaderProvider = authorizationHeaderProvider;
    }

    [HttpPost(Name = "JWT")]
    public async Task<Dictionary<string, string>> PostAsync([FromBody] jwtPayload requestPayload)
    {
        Dictionary<string, string> claims = new Dictionary<string, string>();

        try
        {
            var handler = new JwtSecurityTokenHandler();

            SecurityToken securityToken = handler.ReadToken(requestPayload.token);
            System.IdentityModel.Tokens.Jwt.JwtSecurityToken jwt = ((System.IdentityModel.Tokens.Jwt.JwtSecurityToken)securityToken);

            string[] ignoreClaims = { "exp", "aio", "azpacr", "rh", "uti", "azp", "iat", "sub", "nbf", "acct", "acr", "ipaddr", "platf", "puid", "appid", "tenant_region_scope", "tid", "wids", "xms_st", "xms_tcdt" };
            foreach (var item in jwt.Claims)
            {
                if (requestPayload.showSystemClaims == true)
                    claims.Add(item.Type, item.Value);
                else
                    if (!ignoreClaims.Any(item.Type.Contains))
                    claims.Add(item.Type, item.Value);
            }

        }
        catch (System.Exception ex)
        {
            claims.Add("error", ex.Message);
        }

        return claims.OrderBy(obj => obj.Key).ToDictionary(obj => obj.Key, obj => obj.Value); ;
    }
}

public class jwtPayload
{
    public string token { get; set; }
    public bool? showSystemClaims { get; set; } = false;
}

