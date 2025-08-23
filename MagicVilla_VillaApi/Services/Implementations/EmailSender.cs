using MagicVilla_VillaApi.Services.InterFaces;
using System.Net;
using System.Net.Mail;

namespace MagicVilla_VillaApi.Services.Implementations
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string confirmationLink,int num=1)
        {
            string body;
            if (num == 1)
            {
                body = $@"
<html>
<head>
  <style>
    .button {{
      background-color: #4CAF50;
      border: none;
      color: white;
      padding: 12px 20px;
      text-align: center;
      text-decoration: none;
      display: inline-block;
      font-size: 16px;
      margin: 10px 0;
      cursor: pointer;
      border-radius: 5px;
    }}
  </style>
</head>
<body>
  <h2>مرحبًا بك في موقعنا مقالاتي !</h2>
  <p>شكرًا لتسجيلك. لتفعيل حسابك اضغط الزر التالي:</p>
  <a href='{confirmationLink}' class='button'>فعّل حسابك</a>
  <p>إذا لم يعمل الزر، يمكنك نسخ الرابط التالي ولصقه في المتصفح:</p>
  <p>{confirmationLink}</p>
</body>
</html>";
            }
            else
            {
                body = $@"
<html>
<head>
  <style>
    .button {{
      background-color: #4CAF50;
      border: none;
      color: white;
      padding: 12px 20px;
      text-align: center;
      text-decoration: none;
      display: inline-block;
      font-size: 16px;
      margin: 10px 0;
      cursor: pointer;
      border-radius: 5px;
    }}
  </style>
</head>
<body>
  <h2>مرحبًا بك في موقعنا مقالاتي !</h2>
  <p> . لاعادة تعيين كلمة السر  اضغط علي الزر التالي:</p>
  <a href='{confirmationLink}' class='button'>اعادة تعيين كلمة السر</a>
  <p>إذا لم يعمل الزر، يمكنك نسخ الرابط التالي ولصقه في المتصفح:</p>
  <p>{confirmationLink}</p>
</body>
</html>";

            }





            var smtpClient = new SmtpClient("smtp.gmail.com") // أو أي SMTP تاني
            {
                Port = 587,
                Credentials = new NetworkCredential("mohamedtec1144@gmail.com", "mxjggydrjgzjehzh"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("mohamedtec1144@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }

    }
}
