using System;
using System.Collections.Generic;
using System.Threading;
using Vintagestory.API.Util;

namespace CommonApis.Transact.Api;

public sealed class Transaction : ITransactionContext, IDisposable {

    public int Depth { get; }

    public TransactionLifecycleState LifecycleState { get; private set; }

    private readonly List<ITransactionParticipant> _participants = new();

    private readonly HashSet<ITransactionClosable> _closables = new();

    private bool _wasCommited;

    private readonly ITransactionContext? _parent;

    private readonly Thread _thread;

    private static readonly ThreadLocal<Manager> ManagerInstance = new(() => new Manager());

    public static Transaction OpenRoot() {
        //Ignoring nullability of Value because it can never be null with provided initializer
        var depth = ManagerInstance.Value!.CurrentDepth;
        if (depth != 0) {
            throw new TransactionStackException($"There is an open transaction stack on this thread, calling {nameof(OpenRoot)}() is not allowed", depth);
        }

        return new Transaction(null);
    }

    private Transaction(ITransactionContext? parent) {
        _parent = parent;
        Depth = DepthOf(parent) + 1;

        //Increment current depth
        ManagerInstance.Value!.CurrentDepth++;
        _thread = Thread.CurrentThread;

        _wasCommited = false;
        LifecycleState = TransactionLifecycleState.Open;
    }

    public void RegisterParticipant(ITransactionParticipant participant) {
        AssertOpen();
        AssertCurrentDepth();
        AssertSingleThread();
        _participants.Add(participant);
        if (participant is ITransactionClosable closable) {
            _closables.Add(closable);
        }
    }

    public Transaction OpenNested() {
        AssertOpen();
        AssertCurrentDepth();
        AssertSingleThread();
        return new Transaction(this);
    }

    public void Commit() {
        AssertOpen();
        AssertSingleThread();
        //Not asserting depth, Commit() can be called any time while open
        _wasCommited = true;
    }

    public void Dispose() {
        AssertOpen();
        AssertCurrentDepth();
        AssertSingleThread();
        LifecycleState = TransactionLifecycleState.Closing;
        var state = _wasCommited ? TransactionCloseResult.Commited : TransactionCloseResult.Cancelled;
        foreach (var participant in _participants) {
            participant.Close(state, Depth);
        }
        //If commited handle final close and closable bubbling
        if (_wasCommited) {
            if (_parent != null) {
                if (_parent is not Transaction transaction) {
                    throw new InvalidCastException($"Invalid transaction parent type, creating custom implementations of {nameof(ITransactionContext)} is highly discouraged");
                }
                transaction.AddClosables(_closables);
            }
            else {
                //We are closing the root
                foreach (var closable in _closables) {
                    closable.CloseFinalCommited();
                }   
            }
        }

        LifecycleState = TransactionLifecycleState.Closed;
    }

    private void AddClosables(IEnumerable<ITransactionClosable> closables) {
        _closables.AddRange(closables);
    }

    private static int DepthOf(ITransactionContext? transaction) {
        return transaction?.Depth ?? 0;
    }

    private void AssertOpen() {
        if (LifecycleState != TransactionLifecycleState.Open) {
            throw new TransactionNotOpenException(LifecycleState);
        }
    }

    private void AssertCurrentDepth() {
        AssertStackDepth(Depth);
    }

    private void AssertSingleThread() {
        if (_thread != Thread.CurrentThread) {
            throw new TransactionThreadException("Transactions can only be used on the thread they were created on");
        }
    }
    
    private static void AssertStackDepth(int expectedDepth) {
        var depth = ManagerInstance.Value!.CurrentDepth;
        if (depth != expectedDepth) {
            throw new TransactionStackException($"Expected stack depth to be {expectedDepth}, but was {depth}. Operating on wrong transaction?", depth);
        }
    }

    private class Manager {

        //Maybe will do something more in the future
        public int CurrentDepth { get; set; }

    }
}