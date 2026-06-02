using System;

namespace CommonApis.Transact.Api;

public interface ITransactionContext {
    
    int Depth { get; }

    void RegisterParticipant(ITransactionParticipant participant);

    Transaction OpenNested();

}