namespace Core.BLL.Constants
{
    /// <summary>
    /// Chuẩn hóa Mã Lỗi Nghiệp vụ (Nested Static String Constants) tuân thủ RULE-ARCHITECTURE-AND-SAFETY
    /// </summary>
    public static partial class DomainErrorCodes
    {
        public static class ListView
        {
            public const string ConfigSectionMissing = "LISTVIEW_CONFIG_SECTION_MISSING";
            public const string InvalidPageSize = "LISTVIEW_INVALID_PAGE_SIZE";
            public const string ViewKeyRequired = "LISTVIEW_VIEWKEY_REQUIRED";
        }

        public static class System
        {
            public const string FatalException = "SYS_FATAL_EXCEPTION";
            public const string Unauthorized = "SYS_UNAUTHORIZED";
            public const string Forbidden = "SYS_FORBIDDEN";
            public const string DatabaseTimeout = "SYS_DATABASE_TIMEOUT";
        }

        public static class Common
        {
            public const string NotFound = "COMMON_NOT_FOUND";
            public const string InvalidInput = "COMMON_INVALID_INPUT";
            public const string DuplicateCode = "COMMON_DUPLICATE_CODE";
        }
    }
}
