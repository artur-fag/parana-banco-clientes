using System.Net.Mail;

namespace Clientes.Domain.ValueObjects;

public static class EmailValidator
{
    public static bool EhValido(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        if (email != email.Trim())
            return false;

        try
        {
            var endereco = new MailAddress(email);

            return endereco.Address == email
                && email.Contains('@')
                && email.Split('@')[1].Contains('.');
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
