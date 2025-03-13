using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BitzArt.XDoc;

/// <summary>
/// Configures XML documentation comments for Entity Framework Core entities and their properties.
/// </summary>
[PublicAPI]
public class EntitiesCommentConfigurator
{
    private readonly ILogger _logger;
    private readonly XDoc _xDoc;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public EntitiesCommentConfigurator(XDoc xDoc, ILogger<EntitiesCommentConfigurator> logger)
    {
        _logger = logger;
        _xDoc = xDoc;
    }

    /// <summary>
    /// Configure comments for entities and properties.
    /// </summary>
    /// <param name="modelBuilder"></param>
    public void ConfigureComments(ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes();

        foreach (var entityType in entityTypes)
        {
            var entityComment = _xDoc.Get(entityType.ClrType).ToPlainText();

            // For owned entities, we don't set the comment on the entity itself
            // But we will set the comment on the properties

            var isOwned = entityType.IsOwned();
            var tableName = entityType.GetTableName();

            if (!isOwned && tableName is not null)
            {
                entityType.SetComment(entityComment);
            }

            var properties = entityType.GetProperties();

            foreach (var property in properties)
            {
                var isShadowProperty = property.IsShadowProperty();

                if (isShadowProperty)
                {
                    _logger.LogDebug($"Skipping shadow property [Name: {property.Name}]");

                    continue;
                }

                var propertyInfo = entityType.ClrType.GetProperty(property.Name);

                if (propertyInfo == null)
                {
                    return;
                }
                
                var propertyComment = _xDoc.Get(propertyInfo).ToPlainText();

                property.SetComment(propertyComment);
            }
        }
    }
}