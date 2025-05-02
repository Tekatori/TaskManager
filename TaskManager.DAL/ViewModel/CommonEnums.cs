using System.ComponentModel;

namespace TaskManager.DAL.ViewModel
{
    public static class CommonEnums
    {
        public enum TaskStatus
        {
            [Description("Tạo Mới")]
            New = 0,

            [Description("Đang xử lý")]
            InProgress = 1,

            [Description("Hoàn thành")]
            Completed = 2,

            [Description("Chưa hoàn thành")]
            NotCompleted = -1,

            [Description("Tất cả trạng thái")]
            All = -2
        }
        public enum TaskPriority
        {
            [Description("Thấp")]
            Low = 0,

            [Description("Trung Bình")]
            Medium = 1,

            [Description("Cao")]
            High = 2
        }
        public enum Role
        {
            [Description("Người dùng")]
            User = 0,

            [Description("Quản trị viên")]
            Admin = 1,

            [Description("Quản lý")]
            Leader = 2
        }
        public static string GetDescription(this Enum value)
        {

            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attribute?.Description ?? value.ToString();
        }
    }
}
