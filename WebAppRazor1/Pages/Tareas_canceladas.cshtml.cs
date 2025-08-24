using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WebAppRazor1.Models;

namespace WebAppRazor1.Pages
{
    public class Tareas_canceladasModel : PageModel
    {
        public List<Tarea> Tareas { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; } = 5;

        public void OnGet(int pagina = 1, int tamanoPagina = 5)
        {
            // Ruta al archivo JSON (asegúrate de que exista en tu proyecto)
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

            // Leer el JSON y deserializarlo
            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            var todasLasTareas = JsonSerializer.Deserialize<List<Tarea>>(jsonContent);

            // Agregar un filtro
            todasLasTareas = todasLasTareas
                .Where(t => t.estado == "Cancelado")
                .ToList();

            // Lógica de paginación
            TamanoPagina = tamanoPagina < 1 ? 5 : tamanoPagina;
            PaginaActual = pagina < 1 ? 1 : pagina;
            TotalPaginas = (int)Math.Ceiling(todasLasTareas.Count / (double)TamanoPagina);
            // PAra cuando exceda el número de páginas
            if (PaginaActual > TotalPaginas && TotalPaginas > 0)
            {
                PaginaActual = TotalPaginas;
            }
            Tareas = todasLasTareas.Skip((PaginaActual - 1) * TamanoPagina).Take(TamanoPagina).ToList();
        }
    }
}
