﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BitzArt.XDoc.Tests;

public class EntityTypeBuilderExtensionsTests
{
    private class TestDbContext1(Action<TestDbContext, ModelBuilder> onModelCreating) : TestDbContext(onModelCreating);
    private class TestDbContext2(Action<TestDbContext, ModelBuilder> onModelCreating) : TestDbContext(onModelCreating);
    private class TestDbContext3(Action<TestDbContext, ModelBuilder> onModelCreating) : TestDbContext(onModelCreating);
    private class TestDbContext4(Action<TestDbContext, ModelBuilder> onModelCreating) : TestDbContext(onModelCreating);
    private class TestDbContext5(Action<TestDbContext, ModelBuilder> onModelCreating) : TestDbContext(onModelCreating);
    private class TestDbContext6(Action<TestDbContext, ModelBuilder> onModelCreating) : TestDbContext(onModelCreating);
    private class TestDbContext7(Action<TestDbContext, ModelBuilder> onModelCreating) : TestDbContext(onModelCreating);
    private class TestDbContext8(Action<TestDbContext, ModelBuilder> onModelCreating) : TestDbContext(onModelCreating);
    private class TestDbContext9(Action<TestDbContext, ModelBuilder> onModelCreating) : TestDbContext(onModelCreating);

    [Fact]
    public void HasPropertyComment_WithString_ShouldSetComment()
    {
        // Arrange
        var testComment = "abc";

        var testContext = new TestDbContext1((context, modelBuilder) =>
        {
            modelBuilder
                .Entity<MyFirstClass>()
                .HasPropertyComment(x => x.Id, testComment);
        });

        // Act
        _ = testContext.Model;

        // Assert
        Assert.Equal(testComment, testContext
            .GetService<IDesignTimeModel>()
            .Model
            .FindEntityType(typeof(MyFirstClass))!
            .FindProperty(nameof(MyFirstClass.Id))!
            .GetComment());
    }

    [Fact]
    public void HasPropertyComment_WithSameClassPropertyExpression_ShouldSetComment()
    {
        // Arrange
        var testContext = new TestDbContext2((context, modelBuilder) =>
        {
            var xdoc = new XDoc();

            modelBuilder
                .Entity<MyFirstClass>()
                .HasPropertyComment<MyFirstClass, int, MyFirstClass, int>(xdoc, x => x.Id, x => x.Id);
        });

        // Act
        _ = testContext.Model;

        // Assert
        Assert.Equal(MyFirstClass.IdComment, testContext
            .GetService<IDesignTimeModel>()
            .Model
            .FindEntityType(typeof(MyFirstClass))!
            .FindProperty(nameof(MyFirstClass.Id))!
            .GetComment());
    }

    [Fact]
    public void HasPropertyComment_WithOtherClassPropertyExpression_ShouldSetComment()
    {
        // Arrange
        var testContext = new TestDbContext3((context, modelBuilder) =>
        {
            var xdoc = new XDoc();

            modelBuilder
                .Entity<MyFirstClass>()
                .HasPropertyComment<MyFirstClass, int, MySecondClass, string>(xdoc, x => x.Id, x => x.Name);
        });

        // Act
        _ = testContext.Model;

        // Assert
        Assert.Equal(MySecondClass.NameComment, testContext
            .GetService<IDesignTimeModel>()
            .Model
            .FindEntityType(typeof(MyFirstClass))!
            .FindProperty(nameof(MyFirstClass.Id))!
            .GetComment());
    }

    [Fact]
    public void HasPropertyComment_WithOwnProperty_ShouldSetComment()
    {
        // Arrange
        var testContext = new TestDbContext4((context, modelBuilder) =>
        {
            var xdoc = new XDoc();

            modelBuilder
                .Entity<MyFirstClass>()
                .HasPropertyComment(xdoc, x => x.Id);
        });

        // Act
        _ = testContext.Model;

        // Assert
        Assert.Equal(MyFirstClass.IdComment, testContext
            .GetService<IDesignTimeModel>()
            .Model
            .FindEntityType(typeof(MyFirstClass))!
            .FindProperty(nameof(MyFirstClass.Id))!
            .GetComment());
    }

    [Fact]
    public void HasPropertyComment_WithPropertyNameString_ShouldSetComment()
    {
        // Arrange
        var testContext = new TestDbContext5((context, modelBuilder) =>
        {
            var xdoc = new XDoc();

            modelBuilder
                .Entity<MyFirstClass>()
                .HasPropertyComment(xdoc, nameof(MyFirstClass.Id));
        });

        // Act
        _ = testContext.Model;

        // Assert
        Assert.Equal(MyFirstClass.IdComment, testContext
            .GetService<IDesignTimeModel>()
            .Model
            .FindEntityType(typeof(MyFirstClass))!
            .FindProperty(nameof(MyFirstClass.Id))!
            .GetComment());
    }

    [Fact]
    public void HasPropertyComment_WithPropertyNameStringForProperty_ShouldSetComment()
    {
        // Arrange
        var propertyName = "MyCustomProperty";

        var testContext = new TestDbContext6((context, modelBuilder) =>
        {
            var xdoc = new XDoc();

            modelBuilder
                .Entity<MyFirstClass>()
                .Property<string>(propertyName)
                .HasColumnName(propertyName)
                .HasPropertyComment<MyFirstClass, string, MySecondClass, string?>(xdoc, propertyName, o => o.NullableName)
                .IsRequired()
                .HasColumnType("string");
        });

        // Act
        _ = testContext.Model;

        // Assert
        Assert.Equal(MySecondClass.NullableNameComment, testContext
            .GetService<IDesignTimeModel>()
            .Model
            .FindEntityType(typeof(MyFirstClass))!
            .FindProperty(propertyName)!
            .GetComment());
    }
    
    [Fact]
    public void HasPropertyComment_WithPropertyExpressoin_ShouldSetComment()
    {
        // Arrange
        var propertyName = "MyCustomProperty";

        var testContext = new TestDbContext7((context, modelBuilder) =>
        {
            var xdoc = new XDoc();

            modelBuilder
                .Entity<MyFirstClass>()
                .Property<string>(propertyName)
                .HasColumnName(propertyName)
                .HasPropertyComment(xdoc, (MySecondClass o) => o.NullableName)
                .IsRequired()
                .HasColumnType("string");
        });

        // Act
        _ = testContext.Model;

        // Assert
        Assert.Equal(MySecondClass.NullableNameComment, testContext
            .GetService<IDesignTimeModel>()
            .Model
            .FindEntityType(typeof(MyFirstClass))!
            .FindProperty(propertyName)!
            .GetComment());
    }
}