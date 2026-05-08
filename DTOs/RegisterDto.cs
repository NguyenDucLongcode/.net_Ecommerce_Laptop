using System.ComponentModel.DataAnnotations;

namespace ComChienMaDui.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [MinLength(2, ErrorMessage = "Họ và tên phải có ít nhất 2 ký tự")]
        [MaxLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(256, ErrorMessage = "Email không được vượt quá 256 ký tự")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        [MaxLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Mật khẩu phải bao gồm: ít nhất 8 ký tự, 1 chữ hoa, 1 chữ thường, 1 số và 1 ký tự đặc biệt")]
        public string Password { get; set; } = null!;

    }
}