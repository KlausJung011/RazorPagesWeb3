using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazor1.Models;

namespace WebAppRazor1.Pages
{
    public class IndexModel : PageModel
    {
        public List<Tarea> Tareas { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; } = 5;//Cambiar de manera dinámica este número en un pequeño input

        /*Trabajar index Filtrar pendiente y en curso
         * Añadir el botón crear Tarea: desplegar un formulario para la nueva tarea
         * Link de tareas finalizadas
         * Link tareas canceladas
         */

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(int pagina = 1, string estado = "", int tamanoPagina = 5)
        {
            // Ruta al archivo JSON (asegúrate de que exista en tu proyecto)
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

            // Leer el JSON y deserializarlo
            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            var todasLasTareas = JsonSerializer.Deserialize<List<Tarea>>(jsonContent);

            // Agregar un filtro
            // Filtrar por estado si se recibe
            /*if (!string.IsNullOrWhiteSpace(estado))
            {
                todasLasTareas = todasLasTareas
                    .Where(t => t.estado == estado)
                    .ToList();
            }
            else
            {
                
            }*/

            todasLasTareas = todasLasTareas
                .Where(t => t.estado == "Pendiente" || t.estado == "En curso")
                .ToList();

            // Lógica de paginación
            TamanoPagina = tamanoPagina < 1 ? 5 : tamanoPagina;
            PaginaActual = pagina < 1 ? 1 : pagina;
            TotalPaginas = (int)Math.Ceiling(todasLasTareas.Count / (double)TamanoPagina);
            Tareas = todasLasTareas.Skip((PaginaActual - 1) * TamanoPagina).Take(TamanoPagina).ToList();
        }
    }
}
