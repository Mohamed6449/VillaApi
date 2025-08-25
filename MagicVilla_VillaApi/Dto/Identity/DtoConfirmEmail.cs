namespace MagicVilla_VillaApi.Dto.Identity
{
    public class DtoConfirmEmail
    {
        public string Email { get; set; }

        public string Id { get; set; }

        public bool CanResend { get; set; }

        public TimeSpan TimeRemain { get; set; }
    }
}
