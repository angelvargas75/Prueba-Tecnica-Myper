using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAppMyper3.Models;

public partial class Provincia
{
    [Key]
    public int Id { get; set; }

    public int? IdDepartamento { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? NombreProvincia { get; set; }

    [InverseProperty("IdProvinciaNavigation")]
    public virtual ICollection<Distrito> Distrito { get; set; } = new List<Distrito>();

    [ForeignKey("IdDepartamento")]
    [InverseProperty("Provincia")]
    public virtual Departamento? IdDepartamentoNavigation { get; set; }

    [InverseProperty("IdProvinciaNavigation")]
    public virtual ICollection<Trabajadores> Trabajadores { get; set; } = new List<Trabajadores>();
}
