using System;
using System.Collections.Generic;

namespace CommonApis.Transaction.Api;

public class Transaction : ITransactionContext, IDisposable {

    public int Depth { get; }

    public TransactionLifecycleState LifecycleState { get; private set; }

    private readonly List<ITransactionParticipant> _participants = new();

    private bool _wasCommited;

    private readonly Transaction? _parent;
    
    internal Transaction(Transaction? parent) {
        _parent = parent;
        Depth = DepthOf(parent) + 1;
        _wasCommited = false;
        LifecycleState = TransactionLifecycleState.Open;
    }
    
    public void RegisterParticipant(ITransactionParticipant participant) {
        _participants.Add(participant);
    }

    public void Commit() {
        if (LifecycleState != TransactionLifecycleState.Open) {
            throw new TransactionNotOpenException(LifecycleState);
        }
        _wasCommited = true;
    }
    
    public void Dispose() {
        LifecycleState = TransactionLifecycleState.Closing;
        var state = _wasCommited ? TransactionCloseResult.Commited : TransactionCloseResult.Cancelled;
        foreach (var participant in _participants) {
            participant.Close(state, Depth);
        }
        LifecycleState = TransactionLifecycleState.Closed;
    }

    private static int DepthOf(Transaction? transaction) {
        return transaction?.Depth ?? 0;
    }
}