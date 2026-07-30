using Domain.Entities;
using Infrastructure.Auth;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

public record LoginRequest(string Identifiant, string MotDePasse);
public record RegisterRequest(string NomComplet, string Identifiant, string MotDePasse);


[ApiController]
[Route("api/auth")]
public class AuthController(CallFlowDbContext db, TokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var agent = await db.Agents.FirstOrDefaultAsync(a => a.Identifiant == req.Identifiant);

        if (agent is null || !BCrypt.Net.BCrypt.Verify(req.MotDePasse, agent.MotDePasseHash))
            return Unauthorized("Identifiant ou mot de passe invalide");

        var token = tokenService.GenererToken(agent);
        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var agent = new Agent
        {
            NomComplet = req.NomComplet,
            Identifiant = req.Identifiant,
            MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(req.MotDePasse),
            Role = RoleAgent.Agent,
            Actif = true
        };
        db.Agents.Add(agent);
        await db.SaveChangesAsync();
        return Ok(new { agent.Id });
    }
}