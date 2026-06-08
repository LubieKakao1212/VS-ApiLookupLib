namespace CommonApis.Transact.Api;

public enum TransactionLifecycleState {
    Open,
    Closing,
    Closed
}

public enum TransactionCloseResult {
    Commited,
    Cancelled
}

