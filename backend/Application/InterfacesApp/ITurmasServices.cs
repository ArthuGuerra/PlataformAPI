using Application.DataTransferObject;
using Application.DataTransferObject.AppDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.InterfacesApp
{
    public interface ITurmasServices
    {
        public Task<ICollection<TurmasDTO>> GetTurmas();
        public Task<TurmasDTO> CriarTurma(TurmasDTO turma);
        public Task<bool> RetirarTurma(TurmasDTO turma);
        public Task<bool> RetirarAluno(InscricaoAppDTO dto);
        public Task<TurmasDTO> AtualizarTurma(TurmasDTO turma);
    }
}
