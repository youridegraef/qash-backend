using Application.Domain;

namespace GraphqlAPI.Types {
    public class CategoryType : ObjectType<Category> {
        protected override void Configure(IObjectTypeDescriptor<Category> descriptor) {
            descriptor.Description("Represents a category for transactions.");

            descriptor.Field(c => c.Id).Type<NonNullType<IdType>>();
            descriptor.Field(c => c.Name).Type<NonNullType<StringType>>();
            descriptor.Field(c => c.UserId).Type<NonNullType<IntType>>();
            descriptor
                .Field(c => c.ColorHexCode)
                .Type<NonNullType<StringType>>();
        }
    }
}