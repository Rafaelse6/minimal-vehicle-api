namespace Minimal_Vehicle_API.Domain.ModelViews
{
    public record AdmLogged
    {
        public string Email { get; set; } = default!;

        public string Profile {  get; set; } = default!;

        public string Token { get; set; } = default!;
    }
}
