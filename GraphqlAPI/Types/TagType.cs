namespace GraphqlAPI.Types {
    public class TagType : ObjectType<Application.Domain.Tag> {
        protected override void Configure(IObjectTypeDescriptor<Application.Domain.Tag> descriptor) {
            descriptor.Description("Represents a tag that can be applied to transactions.");

            descriptor.Field(t => t.Id).Type<NonNullType<IdType>>();
            descriptor.Field(t => t.Name).Type<NonNullType<StringType>>();
            descriptor
                .Field(t => t.ColorHexCode)
                .Type<NonNullType<StringType>>();
            descriptor.Field(t => t.UserId).Type<NonNullType<IntType>>();
        }
    }
}