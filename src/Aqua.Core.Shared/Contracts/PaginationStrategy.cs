namespace Shared.GUI.Contracts
{
    /// <summary>
    /// Chiến lược phân trang hỗ trợ bởi Generic Template
    /// </summary>
    public enum PaginationStrategy
    {
        ClientSide = 1, // Tải toàn bộ mảng dữ liệu 1 lần, phân trang & sort trên RAM
        ServerSide = 2  // Đẩy Skip/Take xuống Backend SQL Server (Skill 40)
    }
}

