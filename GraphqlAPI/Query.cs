using Application.Dtos;
using Application.Interfaces;
using HotChocolate;
using HotChocolate.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphqlAPI;

public class Query {
    [GraphQLDescription("Gets a specific user by their ID.")]
    public TransactionDto GetTransactionById(int id, [Service] ITransactionService transactionService) {
        return transactionService.GetById(id);
    }
}