namespace Plataforma_Front.ViewModels
{
    public class TokenViewModel
    {
              
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public bool Authenticated { get; set; }
        public string? Message { get; set; }
    }
}
