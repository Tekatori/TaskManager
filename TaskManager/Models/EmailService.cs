using System.Net.Mail;
using System.Net;

namespace TaskManager.Models
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_config["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_config["EmailSettings:Port"]),
                Credentials = new NetworkCredential(
                    _config["EmailSettings:SenderEmail"],
                    _config["EmailSettings:SenderPassword"]
                ),
                EnableSsl = bool.Parse(_config["EmailSettings:EnableSsl"])
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:SenderEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            smtpClient.Send(mailMessage);
        }
        public string GetHtmlContentEmail(string pResetLink)
        {
            return $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='UTF-8'>
                        <style>
                            body {{
                                font-family: 'Segoe UI', Tahoma, sans-serif;
                                background-color: #f5f7fa;
                                margin: 0;
                                padding: 20px;
                            }}
                            .email-container {{
                                max-width: 520px;
                                margin: 0 auto;
                                background: #ffffff;
                                padding: 30px;
                                border-radius: 12px;
                                box-shadow: 0 4px 20px rgba(0, 0, 0, 0.05);
                                text-align: center;
                            }}
                            .header {{
                                font-size: 26px;
                                color: #6c5ce7;
                                font-weight: bold;
                                margin-bottom: 20px;
                            }}
                            .content {{
                                font-size: 16px;
                                color: #2d3436;
                                line-height: 1.6;
                            }}
                            .btn {{
                                display: inline-block;
                                margin-top: 25px;
                                padding: 12px 26px;
                                background-color: #00cec9;
                                color: #ffffff;
                                text-decoration: none;
                                font-weight: 600;
                                font-size: 16px;
                                border-radius: 6px;
                                transition: background 0.3s ease;
                            }}
                            .btn:hover {{
                                background-color: #01a3a4;
                            }}
                            .footer {{
                                margin-top: 30px;
                                font-size: 13px;
                                color: #636e72;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <div class='header'>🔒 Teka - Task Manager</div>
                            <div class='content'>
                                Xin chào 👋,<br><br>
                                Chúng tôi vừa nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.<br>
                                Nhấn nút bên dưới để tiến hành khôi phục mật khẩu. 🔑<br><br>
                                ⏳ <strong>Lưu ý:</strong> Liên kết có hiệu lực trong <strong>1 giờ</strong>.
                            </div>
                            <a href='{pResetLink}' class='btn'>🚀 Đặt lại mật khẩu</a>
                            <div class='footer'>
                                Nếu không phải bạn gửi yêu cầu này, vui lòng bỏ qua email này.<br>
                                Cảm ơn bạn đã sử dụng <strong>Teka - Task Manager</strong>! 🎉
                            </div>
                        </div>
                    </body>
                    </html>";
        }
    }
}
