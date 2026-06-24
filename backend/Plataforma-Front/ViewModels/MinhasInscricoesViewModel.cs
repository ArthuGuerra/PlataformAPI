using Application.DataTransferObject;

namespace Plataforma_Front.ViewModels
{
    public class MinhasInscricoesViewModel
    {
        public string? UserName { get; set; }
        public IEnumerable<InscricaoDTO> Inscricoes { get; set; }
        = Enumerable.Empty<InscricaoDTO>();
    }
}
