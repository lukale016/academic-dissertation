﻿using MedicalRemoteCommunicationSupport.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalRemoteCommunicationSupport.Controllers;

[Route("api/[controller]/[action]")]
public class AuthController : Controller
{
    private readonly IAuthService auth;

    public AuthController(IAuthService authService)
    {
        auth = authService;
    }

    [HttpPost]
    public async Task<ActionResult<object>> Login([FromBody]Credentials creds)
    {
        try
        {
            return await auth.LogIn(creds);
        }
        catch (ResponseException ex)
        {
            return StatusCode(ex.Status, ex.Message);
        }
    }
}