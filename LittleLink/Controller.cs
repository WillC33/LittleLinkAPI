using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace LittleLink;

/// <summary>
/// Handles creation and retrieval of links
/// </summary>
/// <param name="repository"></param>
[ApiController]
[EnableCors("AllowAll")]
[Route("api")]
public class LinkController(Repository repository) : ControllerBase
{
    /// <summary>
    /// Fetches a link from its hash
    /// </summary>
    /// <param name="id">the hashed url</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public IActionResult Fetch([FromRoute] string id)
    {
        Link? link = repository.Fetch(id);
        return link is null ? NotFound() : Ok(link); //this could potentially return a redirect but the Azure config
        //doesn't seem to allow 301 and I cannot find how to change that...
    }
    
    /// <summary>
    /// Persists the hash/url pair for later retrieval
    /// </summary>
    /// <param name="entity">the Link object</param>
    /// <returns></returns>
    [HttpPost("")]
    public IActionResult Post([FromBody] Link entity)
    {
        bool isSuccessful = repository.Write(entity);
        return isSuccessful ? Created() : BadRequest();
    }
}