using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.Json;
using WebAppRazor1.Models;

namespace WebAppRazor1.Pages
{
    public class IndexModel : PageModel
    {
        public List<Tarea> Tareas { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; } = 5;//Cambiar de manera dinámica este número en un pequeño input
        public string EstadoTarea { get; set; }

        /*Trabajar index Filtrar pendiente y en curso
         * Añadir el botón crear Tarea: desplegar un formulario para la nueva tarea
         * Se pueden editar los datos de las tareas en curso y pendientes: cambiar su estado, nombres o fechas
         * Link de tareas finalizadas
         * Link tareas canceladas
         * 
         */

        // Campos para crear
        [BindProperty] public string? NuevaNombre { get; set; }
        [BindProperty] public DateTime? NuevaFecha { get; set; }

        // Campos para editar
        [BindProperty] public string? EditId { get; set; }
        [BindProperty] public string? EditNombre { get; set; }
        [BindProperty] public DateTime? EditFecha { get; set; }
        [BindProperty] public string? EditEstado { get; set; }

        private readonly ILogger<IndexModel> _logger;
        private string JsonPath => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(int pagina = 1, string estado = "", int tamanoPagina = 5)
        {
            var todasLasTareas = LoadTareas();

            // Asegurar ids (y persistirlos si faltan en el archivo)
            bool idsAgregados = EnsureIds(todasLasTareas);
            if (idsAgregados) SaveTareas(todasLasTareas);

            EstadoTarea = estado;

            /*
            // Ruta al archivo JSON (asegúrate de que exista en tu proyecto)
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");

            // Leer el JSON y deserializarlo
            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            var todasLasTareas = JsonSerializer.Deserialize<List<Tarea>>(jsonContent);*/

            // Agregar un filtro
            // Filtrar por estado si se recibe
            if (!string.IsNullOrWhiteSpace(estado))
            {
                todasLasTareas = todasLasTareas
                    .Where(t => t.estado == estado)
                    .ToList();
            }
            else
            {
                todasLasTareas = todasLasTareas
                .Where(t => t.estado == "Pendiente" || t.estado == "En curso")
                .ToList();
            }

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

        public IActionResult OnPostCreate(int pagina, string? estado, int tamanoPagina)
        {
            if (string.IsNullOrWhiteSpace(NuevaNombre) || NuevaFecha is null)
                return RedirectToPage(new { pagina, estado, tamanoPagina });

            var tareas = LoadTareas();
            EnsureIds(tareas);

            var nueva = new Tarea
            {
                idTarea = Guid.NewGuid().ToString(),
                nombreTarea = NuevaNombre!.Trim(),
                fechaVencimiento = NuevaFecha!.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                estado = "Pendiente"
            };

            tareas.Add(nueva);
            SaveTareas(tareas);

            return RedirectToPage(new { pagina, estado, tamanoPagina });
        }

        public IActionResult OnPostEdit(int pagina, string? estado, int tamanoPagina)
        {
            if (string.IsNullOrWhiteSpace(EditId) ||
                string.IsNullOrWhiteSpace(EditNombre) ||
                EditFecha is null ||
                string.IsNullOrWhiteSpace(EditEstado))
            {
                return RedirectToPage(new { pagina, estado, tamanoPagina });
            }

            var tareas = LoadTareas();
            EnsureIds(tareas);

            var existente = tareas.FirstOrDefault(t => t.idTarea == EditId);
            if (existente is not null)
            {
                existente.nombreTarea = EditNombre!.Trim();
                existente.fechaVencimiento = EditFecha!.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                existente.estado = EditEstado!;
                SaveTareas(tareas);
            }

            return RedirectToPage(new { pagina, estado, tamanoPagina });
        }

        private List<Tarea> LoadTareas()
        {
            if (!System.IO.File.Exists(JsonPath)) return new List<Tarea>();
            var jsonContent = System.IO.File.ReadAllText(JsonPath);
            return JsonSerializer.Deserialize<List<Tarea>>(jsonContent) ?? new List<Tarea>();
        }

        // Devuelve true si se añadieron ids
        private bool EnsureIds(List<Tarea> tareas)
        {
            bool changed = false;
            foreach (var t in tareas)
            {
                if (string.IsNullOrWhiteSpace(t.idTarea))
                {
                    t.idTarea = Guid.NewGuid().ToString();
                    changed = true;
                }
            }
            return changed;
        }

        private void SaveTareas(List<Tarea> tareas)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(tareas, options);
            System.IO.File.WriteAllText(JsonPath, json);
        }
    }
}
