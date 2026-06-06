using System;
using System.Collections.Generic;

namespace TransactApiLib.Transact.Api;

public abstract class TransactionParticipant<TSnapshot> : ITransactionParticipant {

    private readonly Stack<SnapshotEntry> _snapshots = new();

    protected void TakeSnapshot(ITransactionContext ctx) {
        var depth = ctx.Depth;
        var localDepth = _snapshots.Count;

        if (depth == localDepth) {
            return;
        }
        
        if (localDepth > depth) {
            throw new ApplicationException("Invalid Transaction context");
        }
        
        _snapshots.Push(new SnapshotEntry(CreateSnapshot(), depth));
        ctx.RegisterParticipant(this);
    }
    
    public void Close(TransactionCloseResult closeResult, int depth) {
        if (!_snapshots.TryPop(out var snapshot) || snapshot.depth != depth) {
            throw new ApplicationException($"Invalid Transaction context, (No snapshot for requested depth {depth})");
        }
        
        if (closeResult == TransactionCloseResult.Cancelled) {
            RestoreSnapshot(snapshot.data);
        }
    }

    public virtual void CloseFinal(TransactionCloseResult closeResult) { }
    
    protected abstract TSnapshot CreateSnapshot();

    protected abstract void RestoreSnapshot(TSnapshot snapshot);

    private struct SnapshotEntry(TSnapshot data, int depth) {
        public TSnapshot data = data;
        public int depth = depth;
    }
}