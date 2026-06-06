namespace TransactApiLib.Transact.Api;

public interface ITransactionParticipant {

    void Close(TransactionCloseResult closeResult, int depth);
    void CloseFinal(TransactionCloseResult closeResult);

}