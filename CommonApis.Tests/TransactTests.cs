using CommonApis.Transact.Api;

namespace CommonApis.Tests;

using static TransactTestPartials;

public class TransactTests {

    #region Creation and Depth

    public class CreateAndDepth {
    
        [Fact]
        public void DepthRoot() {
            using var transaction = Transaction.OpenRoot();
            Assert.Equal(1, transaction.Depth);
        }

        [Fact]
        public void DepthNested() {
            using var transactionRoot = Transaction.OpenRoot();
            using var transactionNested = transactionRoot.OpenNested();
            Assert.Equal(2, transactionNested.Depth);
            Assert.Equal(1, transactionRoot.Depth);
        }
        
        [Fact]
        public void DepthNestedDouble() {
            using var transactionRoot = Transaction.OpenRoot();
            using var transactionNested1 = transactionRoot.OpenNested();
            using var transactionNested2 = transactionNested1.OpenNested();
            Assert.Equal(3, transactionNested2.Depth);
            Assert.Equal(2, transactionNested1.Depth);
            Assert.Equal(1, transactionRoot.Depth);
        }
        
        [Fact]
        public void MultiRoot() {
            using var transaction = Transaction.OpenRoot();
            Assert.Throws<TransactionStackException>(Transaction.OpenRoot);
        }

        [Fact]
        public void MultiNested() {
            using var root = Transaction.OpenRoot();
            using var nested = root.OpenNested();
            Assert.Throws<TransactionStackException>(root.OpenNested);
        }
        
        [Fact]
        public void MultiNestedAfterDoubleNested() {
            using var root = Transaction.OpenRoot();
            using var nested1 = root.OpenNested();
            using var nested2 = nested1.OpenNested();
            
            Assert.Throws<TransactionStackException>(nested1.OpenNested);
        }
        
    }

    #endregion
    
    #region Threading Safeguards

    public class Threading {
        [Fact]
        public void MultiThreadRoots() {
            var obj = new object();

            using var transaction1 = Transaction.OpenRoot();

            Thread t;

            lock (obj) {
                var t1 = new Thread(() => {
                    using var transaction2 = Transaction.OpenRoot();
                    lock (obj) {

                    }
                });
                t1.Start();
                t = t1;
            }
            t.Join();
        }

        [Fact]
        public void MultiThreadAccess() {
            using var transaction = Transaction.OpenRoot();

            var t = new Thread(() => { Assert.Throws<TransactionThreadException>(transaction.Commit); });
            t.Start();
            t.Join();
        }
    }

    #endregion

    #region Simple Participant and Closable

    public class ParticipantCommits {

        [Fact]
        public void WrongDepth() {
            var par = new SimpleParticipant();
            using (var root = Transaction.OpenRoot()) {
                using var nested = root.OpenNested();

                Assert.Throws<TransactionStackException>(() => par.Increment(root));
            }
        }
        
        [Fact]
        public void Commited() {
            Participant(1, true);
        }

        [Fact]
        public void NotCommited() {
            Participant(0, false);
        }

        [Fact]
        public void NestedNN() {
            ParticipantNested(false, false, [1, 2, 2, 0]);
        }
        
        [Fact]
        public void NestedNC() {
            ParticipantNested(false, true, [1, 2, 3, 0]);
        }
        
        [Fact]
        public void NestedCN() {
            ParticipantNested(true, false, [1, 2, 2, 2]);
        }
        
        [Fact]
        public void NestedCC() {
            ParticipantNested(true, true, [1, 2, 3, 3]);
        }
    }

    public class ParticipantClosable {
        [Fact]
        public void Commited() {
            Closable(true, 2, 2);
        }

        [Fact]
        public void NotCommited() {
            Closable(false, -1, 0);
        }

        [Fact]
        public void BubbleNestedNN() {
            ClosableBubbling(false, false, -1);
        }
        
        [Fact]
        public void BubbleNestedNC() {
            ClosableBubbling(false, true, -1);
        }
        
        [Fact]
        public void BubbleNestedCN() {
            ClosableBubbling(true, false, -1);
        }
        
        [Fact]
        public void BubbleNestedCC() {
            ClosableBubbling(true, true, 2);
        }
    }
    
    #endregion
    
}

public static class TransactTestPartials {
    public static void Participant(int expectedValue, bool commited) {
        var par = new SimpleParticipant();
        using (var transaction = Transaction.OpenRoot()) {
            IncrementAssertCommit(par, transaction, 1, commited);
        }
        Assert.Equal(expectedValue, par.Value);
    }
    
    public static void ParticipantNested(bool commitRoot, bool commitNested, params int[] expectedValues) {
        var par = new SimpleParticipant();
        var idx = 0;
        using (var root = Transaction.OpenRoot()) {
            IncrementAssert(par, root, expectedValues[idx++]);
            using (var nested = root.OpenNested()) {
                IncrementAssertCommit(par, nested, expectedValues[idx++], commitNested);
            }
            IncrementAssertCommit(par, root, expectedValues[idx++], commitRoot);
        }
        Assert.Equal(expectedValues[idx++], par.Value);
        Assert.Equal(expectedValues.Length, idx);
    }
    
    public static void IncrementAssertCommit(SimpleParticipant par, Transaction transaction, int expectedValue, bool commit) {
        IncrementAssert(par, transaction, expectedValue);
        if (commit) {
            transaction.Commit();
        }
    }
    
    public static void IncrementAssert(SimpleParticipant par, Transaction transaction, int expectedValue) {
        par.Increment(transaction);
        Assert.Equal(expectedValue, par.Value);
    }

    public static void Closable(bool commit, int expectedResult, int expectedValue) {
        int result = -1;
        
        var par = new SimpleFinalParticipant {
            CloseCallback = value => {
                if (result != -1) {
                    throw new ArgumentException("Double close detected");
                }
                result = value;
            }
        };

        void Action() {
            using var transaction = Transaction.OpenRoot();
            par.Increment(transaction);
            par.Increment(transaction);
            if (commit) {
                transaction.Commit();
            }
        }

        Action();
        Assert.Equal(expectedResult, result);
        Assert.Equal(expectedValue, par.Value);
    }
    
    public static void ClosableBubbling(bool commitRoot, bool commitNested, int expectedResult) {
        int result = -1;
        
        var par = new SimpleFinalParticipant {
            CloseCallback = value => {
                if (result != -1) {
                    throw new ArgumentException("Double close detected");
                }
                result = value;
            }
        };

        using (var root = Transaction.OpenRoot()) {
            using var nested = root.OpenNested();
            par.Increment(nested);
            par.Increment(nested);
            if (commitNested) {
                nested.Commit();
            }
            if (commitRoot) {
                root.Commit();
            }
        }
        Assert.Equal(expectedResult, result);
    }
}

public class SimpleParticipant : TransactionParticipant<int> {

    public int Value { get; private set; } = 0;

    public int Increment(ITransactionContext ctx) {
        TakeSnapshot(ctx);
        return Value++;
    }
    
    protected override int CreateSnapshot() {
        return Value;
    }

    protected override void RestoreSnapshot(int snapshot) {
        Value = snapshot;
    }
}

public class SimpleFinalParticipant : SimpleParticipant, ITransactionClosable {

    public required Action<int> CloseCallback { get; init; }

    public void CloseFinalCommited() {
        CloseCallback(Value);
    }
}