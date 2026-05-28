namespace CommonApis.Storage.Api;

public readonly struct ResourceStack<TResource>(TResource resource, long amount)
    where TResource : IResource<TResource> {

    public readonly TResource resource = resource;
    public readonly long amount = amount;
}