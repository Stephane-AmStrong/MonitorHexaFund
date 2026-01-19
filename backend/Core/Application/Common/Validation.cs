namespace Application.Common;

public static class Validation
{
    public static class Messages
    {
        public const string IdAlreadyInUse = "{0} with id '{{PropertyValue}}' already exists";
        public const string EntityNotFound = "{0} with id '{{PropertyValue}}' does not exist";
        public const string FieldRequired = "{{PropertyName}} is required";
        public const string FieldAlreadyInUseByAnother = "{0} '{{PropertyValue}}' is already used by another {1}";
        public const string RelationshipAlreadyExists = "A {0} between this {1} '{2}' and {3} '{4}' already exists";
        public const string FieldCannotBeModifiedAfterCreation = "The {0} cannot be modified after creation. Please keep it original value";
        public const string Field1AndField2AlreadyInUse = "{0} '{1}' and {2} '{3}' are already in use";
    }

    public static class Entities
    {
        public const string Alert = "Alert";
        public const string Client = "Client";
        public const string Connection = "Connection";
        public const string Host = "Host";
        public const string AppStatus = "AppStatus";
        public const string App = "App";
    }
}
