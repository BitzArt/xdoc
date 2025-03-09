﻿using System.Reflection;
using System.Xml;

namespace BitzArt.XDoc;

/// <summary>
/// Contains documentation for a member of a <see cref="Type"/>.
/// </summary>
/// <typeparam name="TMember">Type of the member.</typeparam>
public abstract class TypeMemberDocumentation<TMember> : MemberDocumentation
    where TMember : MemberInfo
{
    private readonly TypeDocumentation _declaringTypeDocumentation;

    /// <summary>
    /// The <see cref="Type"/> that declares the member.
    /// </summary>
    public Type DeclaringType => _declaringTypeDocumentation.Type;

    /// <summary>
    /// The <typeparamref name="TMember"/> this documentation if provided for.
    /// </summary>
    public TMember Member { get; private set; }

    internal TypeMemberDocumentation(IXDoc source, TypeDocumentation declaringTypeDocumentation, TMember member, XmlNode node)
        : base(source, node)
    {
        Member = member;
        _declaringTypeDocumentation = declaringTypeDocumentation;
    }
}