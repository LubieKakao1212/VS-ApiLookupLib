using System;
using System.Diagnostics.CodeAnalysis;

namespace TransactApiLib.Storage.Api;

/// <summary>
/// Derived classes should override GetHashCode() to match Equals()
/// </summary>
/// <typeparam name="TResource"></typeparam>
[Experimental("IStorage")]
public interface IResource<TResource> : IEquatable<TResource> where TResource : IResource<TResource> {
    
}