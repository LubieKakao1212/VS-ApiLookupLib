namespace CommonApis.Transact.Api;

/// <summary>
/// Base interfaces for transaction participants, however you should use <see cref="TransactionParticipant{TSnapshot}">TransactionParticipant&lt;TSnapshot&gt;</see> wherever possible. <br/>
/// If you want to have a callback for root close implement also <see cref="ITransactionClosable"/>
/// </summary>
public interface ITransactionParticipant {
    void Close(TransactionCloseResult closeResult, int depth);
}

/// <summary>
/// TODO Missing Documentation
/// </summary>
public interface ITransactionClosable {
    void CloseFinalCommited();
}
