using System.Transactions;
using HotChocolate.Types;
using Application.Dtos;
using Application.Domain;
using Application.Interfaces;

namespace GraphqlAPI.Types;

public class TransactionType : ObjectType<TransactionDto>
{
    protected override void Configure(
        IObjectTypeDescriptor<TransactionDto> descriptor
    )
    {
        descriptor.Description("Represents a financial transaction.");

        descriptor.Field(t => t.Id).Type<NonNullType<IdType>>();
        descriptor.Field(t => t.Description).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.Amount).Type<NonNullType<FloatType>>();
        descriptor.Field(t => t.Date).Type<NonNullType<DateType>>();
        descriptor.Field(t => t.UserId).Type<NonNullType<IntType>>();
        descriptor.Field(t => t.CategoryId).Type<NonNullType<IntType>>();

        descriptor
            .Field(t => t.Category)
            .Type<NonNullType<CategoryType>>()
            .Description("The category associated with this transaction.");

        descriptor
            .Field(t => t.Tags)
            .Type<NonNullType<ListType<NonNullType<TagType>>>>()
            .Description("A list of tags associated with this transaction.");
    }
}