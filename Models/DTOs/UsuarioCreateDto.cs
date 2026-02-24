public class UsuarioCreateDto
{
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;

    // Permite enviar a data de criação
    public DateTime? DataCriacao { get; set; } = null;

    public bool Admin { get; set; } = false;
}
