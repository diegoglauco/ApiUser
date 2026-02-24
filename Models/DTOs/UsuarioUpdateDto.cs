public class UsuarioUpdateDto
{
    public string Login { get; set; } = string.Empty;

    // Apenas alterável por administradores
    public bool Admin { get; set; }
}
