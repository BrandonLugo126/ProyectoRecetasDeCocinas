using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoRecetasDeCocinas.Models
{
    public enum Categorias { Almuerzos,Comidas,Cenas,Postres}
    public class RecetaModel
    {
        public string Nombre { get; set; } = null!;
        public string Imagen { get; set; } = null!;
        public string Procedimiento { get; set; } = null!;
        public string Video { get; set; } = "Sin video demostrativo";
        public Categorias Categoria { get; set; }
    }
}
