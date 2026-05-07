using Microsoft.AspNetCore.Http.Connections;

namespace DeportivoUCN.Models.ValueObjects;

public record Rut
{
    public string Value { get; init; }
    public Rut(string value)
    {   // Validation for empty and null strings
        if (string.IsNullOrWhiteSpace(value)||!value.Contains("-"))
            throw new ArgumentException("Formato de RUT inválido");
        // Validation for RUT format
        if(!Isvalid(value))
            throw new ArgumentException("El RUT ingresado no es valido");        
        Value = value.ToUpper().Replace(".","");
    }

    private bool Isvalid(string rut)
    {
        //todo: validation for rut format
        return true;
    }
}

