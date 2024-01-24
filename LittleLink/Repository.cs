namespace LittleLink;

/// <summary>
/// Db Operation Repository for Links
/// </summary>
/// <param name="context">the Db context</param>
public class Repository(DbContext context)
{
    /// <summary>
    /// Fetches a Link object
    /// </summary>
    /// <param name="id">the hashed url Id</param>
    /// <returns></returns>
    public Link? Fetch(string id)
    {
        return context.GetEntityById(id);
    }
    
    /// <summary>
    /// Adds a new Link object to the database
    /// </summary>
    /// <param name="entity">The hash/url pair</param>
    /// <returns></returns>
    public bool Write(Link entity)
    {
        return context.InsertEntity(entity);
    }
}