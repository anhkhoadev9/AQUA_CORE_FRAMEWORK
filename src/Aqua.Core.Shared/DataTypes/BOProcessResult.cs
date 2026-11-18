using System.Collections.Generic;

namespace Shared.DataTypes
{
    /// <summary>
    /// Return contract chuẩn cho các thao tác Command
    /// </summary>
    public class BOProcessResult
    {
        public bool IsSuccess { get; set; }
        public string? Code { get; set; }
        public Dictionary<string, object> Args { get; set; } = new();

        public static BOProcessResult Success() => new() { IsSuccess = true };

        public static BOProcessResult Failed(string code, Dictionary<string, object>? args = null)
        {
            return new BOProcessResult
            {
                IsSuccess = false,
                Code = code,
                Args = args ?? new Dictionary<string, object>()
            };
        }
    }

    /// <summary>
    /// Return contract chuẩn cho các thao tác Query hoặc Command có trả về Content
    /// </summary>
    public class BOProcessResult<TData> : BOProcessResult
    {
        public TData? Data { get; set; }

        public static BOProcessResult<TData> Success(TData data)
            => new() { IsSuccess = true, Data = data };

        public new static BOProcessResult<TData> Failed(string code, Dictionary<string, object>? args = null)
        {
            return new BOProcessResult<TData>
            {
                IsSuccess = false,
                Code = code,
                Args = args ?? new Dictionary<string, object>()
            };
        }
    }
}
