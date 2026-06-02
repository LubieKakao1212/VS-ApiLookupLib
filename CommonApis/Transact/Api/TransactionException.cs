using System;

namespace CommonApis.Transact.Api;

public class TransactionException : Exception {
    public TransactionException() {
    }

    public TransactionException(string? message) : base(message) {
    }

    public TransactionException(string? message, Exception? innerException) : base(message, innerException) {
    }
}

public class TransactionLifecycleException : TransactionException {
    public TransactionLifecycleState CurrentState { get; set; }

    public TransactionLifecycleException(string? message, TransactionLifecycleState currentState) : base(message) {
        CurrentState = currentState;
    }

    public TransactionLifecycleException(string? message, Exception? innerException, TransactionLifecycleState currentState) : base(message, innerException) {
        CurrentState = currentState;
    }
}

public class TransactionNotOpenException(TransactionLifecycleState currentState)
    : TransactionLifecycleException($"Expected transaction to be {nameof(TransactionLifecycleState.Open)} but was ${currentState}", currentState);

public class TransactionStackException : TransactionException {

    public int StackDepth { get; set; }
    
    public TransactionStackException(string? message, int depth) : base(message) {
        StackDepth = depth;
    }

    public TransactionStackException(string? message, Exception? innerException, int depth) : base(message, innerException) {
        StackDepth = depth;
    }
}

public class TransactionThreadException : TransactionException {
    public TransactionThreadException(string? message) : base(message) {
    }

    public TransactionThreadException(string? message, Exception? innerException) : base(message, innerException) {
    }
}
