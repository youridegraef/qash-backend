using Application.Domain;
using Application.Interfaces;

namespace TestLayer.MockData
{
    public class TransactionRepositoryMock : ITransactionRepository
    {
        private readonly List<Transaction> _transactions =
        [
            new Transaction(1, "Boodschappen", 100.0, DateOnly.FromDateTime(DateTime.Today), 1, 1),
            new Transaction(2, "Huur", -50.0, DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), 1, 2),
            new Transaction(3, "Salaris", 200.0, DateOnly.FromDateTime(DateTime.Today.AddDays(-2)), 2, 1)
        ];

        public List<Transaction> FindAll()
        {
            return _transactions.ToList();
        }

        public Transaction FindById(int id)
        {
            return _transactions.FirstOrDefault(t => t.Id == id)!;
        }

        public List<Transaction> FindByUserId(int userId)
        {
            return _transactions.Where(t => t.UserId == userId).ToList();
        }

        public int Add(Transaction transaction)
        {
            int newId = _transactions.Any() ? _transactions.Max(t => t.Id) + 1 : 1;
            // Gebruik de juiste constructor en kopieer de properties
            var newTransaction = new Transaction(
                newId,
                transaction.Description,
                transaction.Amount,
                transaction.Date,
                transaction.UserId,
                transaction.CategoryId
            );
            _transactions.Add(newTransaction);
            return newId;
        }

        public bool Edit(Transaction transaction)
        {
            var index = _transactions.FindIndex(t => t.Id == transaction.Id);
            if (index == -1) return false;

            var updatedTransaction = new Transaction(
                transaction.Id,
                transaction.Description,
                transaction.Amount,
                transaction.Date,
                transaction.UserId,
                transaction.CategoryId
            );
            _transactions[index] = updatedTransaction;
            return true;
        }

        public bool Delete(Transaction transaction)
        {
            return _transactions.RemoveAll(t => t.Id == transaction.Id) > 0;
        }
    }
}