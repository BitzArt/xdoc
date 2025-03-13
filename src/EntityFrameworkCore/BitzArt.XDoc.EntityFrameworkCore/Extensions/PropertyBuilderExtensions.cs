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
    /// Adds a comment to a property of the entity type being configured, using documentation from XDoc.
    /// The comment is extracted from the same property that is being configured.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being configured</typeparam>
    /// <typeparam name="TProperty">The type of the property being configured</typeparam>
    /// <param name="entityTypeBuilder">The builder for the entity type</param>
    /// <param name="xdoc">The XDoc instance used to extract documentation</param>
    /// <param name="propertyExpression">An expression that identifies the property to configure</param>
    /// <returns>The same entity type builder instance so that multiple calls can be chained</returns>
    public static EntityTypeBuilder<TEntity> HasPropertyComment<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        XDoc xdoc,
        Expression<Func<TEntity, TProperty>> propertyExpression)
        where TEntity : class
        => entityTypeBuilder.HasPropertyComment(xdoc, propertyExpression, propertyExpression);

    /// <summary>
    /// Adds a comment to a property of the entity type being configured, using documentation from XDoc.
    /// The comment is extracted from a potentially different property or entity than the one being configured.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being configured</typeparam>
    /// <typeparam name="TProperty">The type of the property being configured</typeparam>
    /// <typeparam name="TCommentTargetEntity">The entity type containing the property whose documentation will be used</typeparam>
    /// <typeparam name="TCommentTargetProperty">The type of the property whose documentation will be used</typeparam>
    /// <param name="entityTypeBuilder">The builder for the entity type</param>
    /// <param name="xdoc">The XDoc instance used to extract documentation</param>
    /// <param name="propertyExpression">An expression that identifies the property to configure</param>
    /// <param name="commentTargetPropertyExpression">An expression that identifies the property whose documentation will be used</param>
    /// <returns>The same entity type builder instance so that multiple calls can be chained</returns>
    /// <exception cref="InvalidOperationException">Thrown when the property name is null or empty</exception>
    public static EntityTypeBuilder<TEntity> HasPropertyComment<TEntity, TProperty, TCommentTargetEntity,
        TCommentTargetProperty>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        XDoc xdoc,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        Expression<Func<TCommentTargetEntity, TCommentTargetProperty>> commentTargetPropertyExpression)
        where TEntity : class
        where TCommentTargetEntity : class
    {
        var comment = GetComment(xdoc, commentTargetPropertyExpression);

        return entityTypeBuilder.HasPropertyComment(propertyExpression, comment);
    }

    /// <summary>
    /// Adds a comment to a property of the entity type being configured, using documentation from XDoc.
    /// The comment is extracted from a property with the specified name in the entity type being configured.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being configured</typeparam>
    /// <param name="entityTypeBuilder">The builder for the entity type</param>
    /// <param name="xdoc">The XDoc instance used to extract documentation</param>
    /// <param name="propertyName">The name of the property to configure</param>
    /// <returns>The same entity type builder instance so that multiple calls can be chained</returns>
    public static EntityTypeBuilder<TEntity> HasPropertyComment<TEntity>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        XDoc xdoc,
        string propertyName)
        where TEntity : class
        => entityTypeBuilder.HasPropertyComment(xdoc, typeof(TEntity), propertyName);

    /// <summary>
    /// Adds a comment to a property of the entity type being configured, using documentation from XDoc.
    /// The comment is extracted from a property with the specified name in the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being configured</typeparam>
    /// <param name="entityTypeBuilder">The builder for the entity type</param>
    /// <param name="xdoc">The XDoc instance used to extract documentation</param>
    /// <param name="targetEntityType">The entity type containing the property whose documentation will be used</param>
    /// <param name="propertyName">The name of the property to configure</param>
    /// <returns>The same entity type builder instance so that multiple calls can be chained</returns>
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

    /// <summary>
    /// Adds a comment to a property of the entity type being configured.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being configured</typeparam>
    /// <typeparam name="TProperty">The type of the property being configured</typeparam>
    /// <param name="entityTypeBuilder">The builder for the entity type</param>
    /// <param name="propertyExpression">An expression that identifies the property to configure</param>
    /// <param name="comment">The comment text to be added to the property</param>
    /// <returns>The same entity type builder instance so that multiple calls can be chained</returns>
    public static EntityTypeBuilder<TEntity> HasPropertyComment<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        string? comment)
        where TEntity : class
    {
        entityTypeBuilder.Property(propertyExpression).HasComment(comment);

        return entityTypeBuilder;
    }

    /// <summary>
    /// Adds a comment to a property being configured, using documentation from XDoc.
    /// The comment is extracted from a property defined in the provided expression.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being configured</typeparam>
    /// <typeparam name="TProperty">The type of the property being configured</typeparam>
    /// <typeparam name="TCommentTargetEntity">The entity type containing the property whose documentation will be used</typeparam>
    /// <typeparam name="TCommentTargetProperty">The type of the property whose documentation will be used</typeparam>
    /// <param name="propertyBuilder">The builder for the property</param>
    /// <param name="xdoc">The XDoc instance used to extract documentation</param>
    /// <param name="propertyName">The name of the property to configure</param>
    /// <param name="commentTargetPropertyExpression">An expression that identifies the property whose documentation will be used</param>
    /// <returns>The same property builder instance so that multiple calls can be chained</returns>
    public static PropertyBuilder<TProperty> HasPropertyComment<TEntity, TProperty, TCommentTargetEntity,
        TCommentTargetProperty>(
        this PropertyBuilder<TProperty> propertyBuilder,
        XDoc xdoc,
        string propertyName,
        Expression<Func<TCommentTargetEntity, TCommentTargetProperty>> commentTargetPropertyExpression)
        where TEntity : class
        where TCommentTargetEntity : class
    {
        var comment = GetComment(xdoc, commentTargetPropertyExpression);

        return propertyBuilder.HasComment(comment);
    }

    /// <summary>
    /// Adds a comment to a property being configured, using documentation from XDoc.
    /// The comment is extracted from a property with the specified name in the target entity type.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being configured</typeparam>
    /// <typeparam name="TCommentTargetEntity">The entity type containing the property whose documentation will be used</typeparam>
    /// <typeparam name="TCommentTargetProperty">The type of the property in the expression parameter (not directly used)</typeparam>
    /// <param name="propertyBuilder">The builder for the property</param>
    /// <param name="xdoc">The XDoc instance used to extract documentation</param>
    /// <param name="commentTargetPropertyExpression">An expression that identifies a property (note: this parameter is not used in the method body)</param>
    /// <returns>The same property builder instance so that multiple calls can be chained</returns>
    public static PropertyBuilder<TProperty> HasPropertyComment<TProperty, TCommentTargetEntity,
        TCommentTargetProperty>(
        this PropertyBuilder<TProperty> propertyBuilder,
        XDoc xdoc,
        Expression<Func<TCommentTargetEntity, TCommentTargetProperty>> commentTargetPropertyExpression)
    {
        var operand = commentTargetPropertyExpression.Body as MemberExpression;
        var targetPropertyName = operand!.Member.Name;

        var comment = GetComment<TCommentTargetEntity>(xdoc, targetPropertyName);

        return propertyBuilder.HasComment(comment);
    }

    /// <summary>
    /// Extracts documentation comment from a property specified in an expression.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being configured</typeparam>
    /// <typeparam name="TCommentTargetEntity">The entity type containing the property whose documentation will be used</typeparam>
    /// <typeparam name="TCommentTargetProperty">The type of the property whose documentation will be used</typeparam>
    /// <param name="xdoc">The XDoc instance used to extract documentation</param>
    /// <param name="commentTargetPropertyExpression">An expression that identifies the property whose documentation will be used</param>
    /// <returns>The extracted documentation comment as plain text</returns>
    /// <exception cref="InvalidOperationException">Thrown when the property name cannot be determined or is empty</exception>
    private static string GetComment<TCommentTargetEntity, TCommentTargetProperty>(
        XDoc xdoc,
        Expression<Func<TCommentTargetEntity, TCommentTargetProperty>> commentTargetPropertyExpression)
        where TCommentTargetEntity : class
    {
        var operand = commentTargetPropertyExpression.Body as MemberExpression;
        var targetPropertyName = operand!.Member.Name;

        if (string.IsNullOrWhiteSpace(targetPropertyName))
        {
            throw new InvalidOperationException($"Property with name '{targetPropertyName}' was not found.");
        }

        var comment = GetComment<TCommentTargetEntity>(xdoc, targetPropertyName);

        return comment;
    }

    /// <summary>
    /// Extracts documentation comment from a property with the specified name in the given entity type.
    /// </summary>
    /// <typeparam name="TCommentTargetEntity">The entity type containing the property whose documentation will be used</typeparam>
    /// <param name="xdoc">The XDoc instance used to extract documentation</param>
    /// <param name="propertyName">The name of the property whose documentation will be extracted</param>
    /// <returns>The extracted documentation comment as plain text</returns>
    private static string GetComment<TCommentTargetEntity>(XDoc xdoc, string propertyName)
    {
        var propertyInfo = typeof(TCommentTargetEntity).GetProperty(propertyName);

        var comment = xdoc.Get(propertyInfo).ToPlainText();

        return comment;
    }
}