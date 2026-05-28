using System;

namespace CommonApis.Transaction.Api;

public interface ITransactionContext {
    
    int Depth { get; }

    void RegisterParticipant(ITransactionParticipant participant);
    
}