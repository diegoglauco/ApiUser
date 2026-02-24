using BCrypt.Net;

public static class PasswordHelper
{
    public static string HashSenha(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }

    public static bool VerificarSenha(string senha, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}