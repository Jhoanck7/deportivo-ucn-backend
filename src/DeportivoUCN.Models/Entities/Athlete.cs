
using System;
using System.Collections.Generic;
using System.Text;
namespace DeportivoUCN.Models.Entities;

public class Athlete
{
    public Guid Id { get; private set;} = Guid.CreateVersion7();    
    
}