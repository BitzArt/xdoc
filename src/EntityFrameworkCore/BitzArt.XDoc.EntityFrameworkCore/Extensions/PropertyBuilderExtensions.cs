using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace BitzArt.XDoc;

/// <summary>
/// Extension methods for PropertyBuilder.
/// </summary>
[PublicAPI]
public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Map property comment from XML documentation to the property.
    /// </summary>
    /// <param name="builder">
    /// Property builder.
    /// </param>
    /// <param name="xdoc">
    /// XDoc instance to use.
    /// </param>
    /// <param name="propertyExpression">
    /// Expression to get the property name.
    /// </param>
    /// <typeparam name="TTargetEntity">
    /// Entity type where the property is defined.
    /// </typeparam>
    /// <typeparam name="TTargetProperty">
    /// Target property type.
    /// </typeparam>
    /// <returns></returns>
    /*public static PropertyBuilder HasXmlComment<TTargetEntity, TTargetProperty>(
        this PropertyBuilder builder,
        XDoc xdoc,
        Expression<Func<TTargetEntity, TTargetProperty>> propertyExpression)
    {
        var type = typeof(TTargetEntity);

        //var expression = propertyExpression.Body as UnaryExpression;
        //var operand = expression?.Operand as MemberExpression;

        var operand = propertyExpression.Body as MemberExpression;
        var propertyName = operand?.Member.Name;

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return builder;
        }

        var propertyInfo = type.GetProperty(propertyName);

        var comment = xdoc.Get(propertyInfo).ToPlainText();

        if (!string.IsNullOrWhiteSpace(comment))
        {
            builder.HasComment(comment);
        }

        return builder;
    }*/

    public static EntityTypeBuilder<TEntity> HasPropertyComment<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        XDoc xdoc,
        Expression<Func<TEntity, TProperty>> propertyExpression)
        where TEntity : class
        => entityTypeBuilder.HasPropertyComment(xdoc, propertyExpression, propertyExpression);

    public static EntityTypeBuilder<TEntity> HasPropertyComment<TEntity, TProperty, TCommentTargetEntity, TCommentTargetProperty>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        XDoc xdoc,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        Expression<Func<TCommentTargetEntity, TCommentTargetProperty>> commentTargetPropertyExpression)
        where TEntity : class
        where TCommentTargetEntity : class
    {
        var operand = commentTargetPropertyExpression.Body as MemberExpression;
        var propertyName = operand!.Member.Name;

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new InvalidOperationException($"Property with name '{propertyName}' was not found.");
        }

        var commentTargetType = typeof(TCommentTargetEntity);
        var targetPropertyInfo = commentTargetType.GetProperty(propertyName);

        var comment = xdoc.Get(targetPropertyInfo).ToPlainText();

        return entityTypeBuilder.HasPropertyComment(propertyExpression, comment);
    }

    public static EntityTypeBuilder<TEntity> HasPropertyComment<TEntity>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        XDoc xdoc,
        string propertyName)
        where TEntity : class
        => entityTypeBuilder.HasPropertyComment(xdoc, typeof(TEntity), propertyName);

    public static EntityTypeBuilder<TEntity> HasPropertyComment<TEntity>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        XDoc xdoc,
        Type targetEntityType,
        string propertyName)
        where TEntity : class
    {
        var propertyInfo = targetEntityType.GetProperty(propertyName);

        var comment = xdoc.Get(propertyInfo).ToPlainText();

        entityTypeBuilder.Property(propertyName).HasComment(comment);

        return entityTypeBuilder;
    }

    public static EntityTypeBuilder<TEntity> HasPropertyComment<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        string? comment)
        where TEntity : class
    {
        entityTypeBuilder.Property(propertyExpression).HasComment(comment);
        return entityTypeBuilder;
    }
}