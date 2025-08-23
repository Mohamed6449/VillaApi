namespace MagicVilla_VillaApi.Services.InterFaces
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage, int num);

    }
}
