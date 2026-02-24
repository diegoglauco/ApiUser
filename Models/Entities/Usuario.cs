using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace UsuariosApi.Models
{
    public class Usuario
    {
        [Key] 
        [Column("usu_codigo")] // Nome da coluna no banco
        public int Id { get; set; }

        [Column("usu_login")]        
        public string Login { get; set; } = string.Empty;
        [Column("usu_senha")]
        public string Senha { get; set; } = string.Empty;
        [Column("usu_admin")]
        public bool Admin { get; set; }
        [Column("usu_datacriacao")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

         
    }
}
