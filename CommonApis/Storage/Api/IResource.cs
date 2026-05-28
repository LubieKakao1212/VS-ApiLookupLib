using System;

namespace CommonApis.Storage.Api;

/// <summary>
/// Derived classes should override GetHashCode() to match Equals()
/// </summary>
/// <typeparam name="TResource"></typeparam>
public interface IResource<TResource> : IEquatable<TResource> where TResource : IResource<TResource> {
    
}