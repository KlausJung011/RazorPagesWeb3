using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazor1.Models;

namespace WebAppRazor1.Pages
{
    public class Tareas_finalizadasModel : PageModel
    {
        public List<Tarea> Tareas { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; } = 5;

        public void OnGet(int pagina = 1, int tamanoPagina = 5)
        {
            // Ruta al archivo JSON (aseg�rate de que exista en tu proyecto)
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

            // Leer el JSON y deserializarlo
            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            var todasLasTareas = JsonSerializer.Deserialize<List<Tarea>>(jsonContent);

            // Agregar un filtro
            todasLasTareas = todasLasTareas
                .Where(t => t.estado == "Finalizado")
                .ToList();

            // L�gica de paginaci�n
            TamanoPagina = tamanoPagina < 1 ? 5 : tamanoPagina;
            PaginaActual = pagina < 1 ? 1 : pagina;
            TotalPaginas = (int)Math.Ceiling(todasLasTareas.Count / (double)TamanoPagina);
            // PAra cuando exceda el n�mero de p�ginas
            if (PaginaActual > TotalPaginas && TotalPaginas > 0)
            {
                PaginaActual = TotalPaginas;
            }
            Tareas = todasLasTareas.Skip((PaginaActual - 1) * TamanoPagina).Take(TamanoPagina).ToList();
        }
    }
}
