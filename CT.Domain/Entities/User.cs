namespace CT.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool DebeRestablecerPassword { get; set; } = false;

    public User(string nombre, string apellido, string email)
    {
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
    }

    public User() { }
}
