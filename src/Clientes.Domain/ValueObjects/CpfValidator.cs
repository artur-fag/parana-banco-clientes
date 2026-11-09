
namespace Clientes.Domain.ValueObjects;

public static class CpfValidator
{

    public static bool EhValido(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        string numeros;

        if (cpf.Length == 11 && cpf.All(c => c >= '0' && c <= '9'))
        {
            numeros = cpf;
        }
        else if (
            cpf.Length == 14 &&
            cpf[3] == '.' &&
            cpf[7] == '.' &&
            cpf[11] == '-')
        {
            numeros = cpf.Replace(".", "").Replace("-", "");

            if (!numeros.All(c => c >= '0' && c <= '9'))
                return false;
        }
        else
        {
            return false;
        }

        if (numeros.Distinct().Count() == 1)
            return false;

        int primeiroDigito = CalcularDigito(numeros, 9);
        int segundoDigito = CalcularDigito(numeros, 10);

        return primeiroDigito == numeros[9] - '0'
            && segundoDigito == numeros[10] - '0';
    }

    private static int CalcularDigito(string cpf, int quantidade)
    {
        int soma = 0;

        for (int i = 0; i < quantidade; i++)
        {
            soma += (cpf[i] - '0') * (quantidade + 1 - i);
        }

        int resto = soma % 11;

        return resto < 2 ? 0 : 11 - resto;
    }
}
