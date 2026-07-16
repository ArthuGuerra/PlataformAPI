using Application.DataTransferObject.IdentityDTO;
using Domain.AppEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DataTransferObject.AppDTO
{
    public class InscricaoAppDTO
    {
       public int Id { get; set; }
       public UsuarioPrintDTO? Usuario {  get; set; }
       public TurmasDTO? Turmas {  get; set; }
    }
}
