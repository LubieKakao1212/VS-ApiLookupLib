using System;
using System.Diagnostics.CodeAnalysis;

namespace TransactApiLib.Storage.Api;

[Experimental("IStorage")]
public readonly struct ResourceStack<TResource>(TResource resource, long amount)
    where TResource : IResource<TResource> {

    public static readonly ResourceStack<TResource> Empty = new();
    
    public delegate void ResourceStackAction(in ResourceStack<TResource> stack);
    
    public readonly TResource? resource = resource;
    public readonly long amount = amount;

    [MemberNotNullWhen(false, nameof(resource))]
    public bool IsEmpty => resource == null || amount == 0;

    public void IfNonEmpty(ResourceStackAction action) {
        if (!IsEmpty) {
            action(this);
        }
    }
    
}