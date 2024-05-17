using Microsoft.Maui.Controls.Shapes;
using ProyectoRecetasDeCocinas.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProyectoRecetasDeCocinas.ViewModels
{
    public class RecetarioViewModel : INotifyPropertyChanged
    {
        public List<RecetaModel> ListaPrincipal { get; set; } = new();
        public ObservableCollection<RecetaModel> ListaMostrar { get; set; } = new();
        int indiceRecetaEditar;
        public IEnumerable<Categorias> ListaCategorias => Enum.GetValues(typeof(Categorias)).Cast<Categorias>();
        public RecetaModel Receta { get; set; } = new();
        public Categorias CategoriaGuardada { get; set; } = new();
        public string Error { get; set; } = null!;
        ObservableCollection<RecetaModel> RecetasAlmuerzo =>
        new ObservableCollection<RecetaModel>(ListaPrincipal.Where(x => x.Categoria == Categorias.Almuerzos)); public IEnumerable<RecetaModel> RecetasAlmuerzos => ListaPrincipal.Select(x => x).Where(x => x.Categoria == Categorias.Almuerzos);
        ObservableCollection<RecetaModel> RecetasPostre =>
        new ObservableCollection<RecetaModel>(ListaPrincipal.Where(x => x.Categoria == Categorias.Postres)); public IEnumerable<RecetaModel> RecetasPostres => ListaPrincipal.Select(x => x).Where(x => x.Categoria == Categorias.Postres);

        ObservableCollection<RecetaModel> RecetasCena =>
        new ObservableCollection<RecetaModel>(ListaPrincipal.Where(x => x.Categoria == Categorias.Cenas)); public IEnumerable<RecetaModel> RecetasCenas => ListaPrincipal.Select(x => x).Where(x => x.Categoria == Categorias.Cenas);
        ObservableCollection<RecetaModel> RecetasComida =>
        new ObservableCollection<RecetaModel>(ListaPrincipal.Where(x => x.Categoria == Categorias.Comidas)); public IEnumerable<RecetaModel> RecetasComidas => ListaPrincipal.Select(x => x).Where(x => x.Categoria == Categorias.Comidas);
        public ICommand VerCategoriasCommand { get; set; }
        public ICommand RegresarAListasCommand { get; set; }
        public ICommand VerAgregarCommand { get; set; }
        public ICommand VerEditarCommand { get; set; }
        public ICommand EditarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public ICommand VerEliminarCommand { get; set; }
        public ICommand VerRecetaCommand { get; set; }
        public ICommand AgregarCommand { get; set; }
        public ICommand SeleccionarCategoriaCommand { get; set; }

        public RecetarioViewModel()
        {
            EliminarCommand = new Command(Eliminar);
            VerEliminarCommand = new Command<RecetaModel>(VerEliminar);
            EditarCommand = new Command(Editar);
            VerEditarCommand = new Command<RecetaModel>(VerEditar);
            VerRecetaCommand = new Command<RecetaModel>(VerReceta);
            RegresarAListasCommand = new Command(VerListas);
            VerAgregarCommand = new Command(VerAgregar);
            AgregarCommand = new Command(AgregarReceta);
            VerCategoriasCommand = new Command(VerCategorias);
            SeleccionarCategoriaCommand = new Command<string>(SeleccionarCategoria);
            Cargar();
        }

        private void Eliminar()
        {
            if (Receta != null)
            {
                ListaPrincipal.Remove(Receta);
                ListaMostrar.Remove(Receta);

                Guardar();
                VerListas();
            }
        }

        private void VerEliminar(RecetaModel receta)
        {
            Receta = receta;
            Error = "";
            Actualizar(nameof(Receta));
            Actualizar(nameof(Error));
            Shell.Current.GoToAsync("//Eliminar");
        }

        private void Editar(object obj)
        {

            if (Receta != null)
            {
                Error = "";
                if (string.IsNullOrWhiteSpace(Receta.Nombre))
                {
                    Error = "Escriba el nombre de la receta.";
                }
                if (string.IsNullOrWhiteSpace(Receta.Procedimiento))
                {
                    Error = "Escriba el procedimiento";
                }
                if (string.IsNullOrWhiteSpace(Receta.Imagen) || !Receta.Imagen.StartsWith("http")
                     || !Receta.Imagen.EndsWith(".jpg"))
                {
                    Error = "Escriba la dirección de una imagen en JPG.";
                }
                if (Receta.Categoria == Categorias.Almuerzos)
                {
                    ListaMostrar = RecetasAlmuerzo;
                    CategoriaGuardada = Categorias.Almuerzos;
                }
                if (Receta.Categoria == Categorias.Comidas)
                {
                    ListaMostrar = RecetasComida;
                    CategoriaGuardada = Categorias.Comidas;
                }
                if (Receta.Categoria == Categorias.Cenas)
                {
                    ListaMostrar = RecetasCena;
                    CategoriaGuardada = Categorias.Cenas;
                }
                if (Receta.Categoria == Categorias.Postres)
                {
                    ListaMostrar = RecetasPostre;
                    CategoriaGuardada = Categorias.Postres;
                }

                if (!string.IsNullOrWhiteSpace(Error))
                {

                    return;
                }
                Actualizar(nameof(Error));
                if (Error == "")
                {
                    ListaPrincipal[indiceRecetaEditar] = Receta;
                    ListaMostrar[indiceRecetaEditar] = Receta;
                    Guardar();
                    VerListas();
                }
            }
            Actualizar(nameof(Receta));
            Actualizar(nameof(ListaPrincipal));
            Actualizar(nameof(ListaMostrar));
        }

        private void VerEditar(RecetaModel receta)
        {
            var clon = new RecetaModel
            {
                Imagen = receta.Imagen,
                Video = receta.Video,
                Categoria = receta.Categoria,
                Nombre = receta.Nombre,
                Procedimiento = receta.Procedimiento
            };
            Receta = clon;
            indiceRecetaEditar = ListaPrincipal.IndexOf(receta);
            Error = "";
            PropertyChanged?.Invoke(this, new(nameof(Receta)));
            PropertyChanged?.Invoke(this, new(nameof(Error)));
            Shell.Current.GoToAsync("//Editar");
        }

        private void VerReceta(RecetaModel receta)
        {
            Receta = receta;
            Actualizar(nameof(Receta));
            Shell.Current.GoToAsync("//Receta");
        }

        private void VerListas()
        {
            SeleccionarCategoria(nameof(CategoriaGuardada));
            Actualizar(nameof(Receta));
            Actualizar(nameof(ListaMostrar));
            Shell.Current.GoToAsync("//Listas");

        }

        private void VerAgregar(object obj)
        {
            Receta = new();
            Actualizar(nameof(ListaMostrar));
            Shell.Current.GoToAsync("//Agregar");
        }

        private void VerCategorias(object obj)
        {
            Shell.Current.GoToAsync("//Categorias");
        }

        public void SeleccionarCategoria(string categoria)
        {

            if (categoria == "Almuerzos")
            {
                ListaMostrar = RecetasAlmuerzo;
                CategoriaGuardada = Categorias.Almuerzos;
            }
            if (categoria == "Comidas")
            {
                ListaMostrar = RecetasComida;
                CategoriaGuardada = Categorias.Comidas;
            }
            if (categoria == "Cenas")
            {
                ListaMostrar = RecetasCena;
                CategoriaGuardada = Categorias.Cenas;
            }
            if (categoria == "Postres")
            {
                ListaMostrar = RecetasPostre;
                CategoriaGuardada = Categorias.Postres;
            }

            Shell.Current.GoToAsync("//Listas");
            Actualizar(nameof(ListaMostrar));
        }
        private void AgregarReceta()
        {
            if (Receta != null)
            {
                Error = "";
                if (string.IsNullOrWhiteSpace(Receta.Nombre))
                {
                    Error = "Escriba el nombre de la receta.";
                }
                if (string.IsNullOrWhiteSpace(Receta.Procedimiento))
                {
                    Error = "Escriba el procedimiento";
                }
                if (string.IsNullOrWhiteSpace(Receta.Imagen) || !Receta.Imagen.StartsWith("http")
                     || !Receta.Imagen.EndsWith(".jpg"))
                {
                    Error = "Escriba la dirección de una imagen en JPG.";
                }

                if (!string.IsNullOrWhiteSpace(Error))
                {
                    Actualizar(nameof(Error));
                    Actualizar(nameof(ListaMostrar));
                    Actualizar(nameof(ListaPrincipal));
                    return;
                }
                ListaPrincipal.Add(Receta);
                Guardar();
                Actualizar(nameof(ListaMostrar));
                Actualizar(nameof(ListaPrincipal));
                SeleccionarCategoria(nameof(Receta.Categoria));
                Shell.Current.GoToAsync("//Listas");
            }
        }

        private void Guardar()
        {
            var ruta = FileSystem.AppDataDirectory;
            File.WriteAllText(ruta + "/Recetas.json", JsonSerializer.Serialize(ListaPrincipal));
        }
        private void Cargar()
        {
            var ruta = FileSystem.AppDataDirectory;
            if (File.Exists(ruta + "/Recetas.json"))
            {
                var datos = JsonSerializer.Deserialize<List<RecetaModel>>(File.ReadAllText(ruta + "/Recetas.json"));
                if (datos != null)
                {
                    ListaPrincipal.AddRange(datos);
                }
            }
        }


        private void Actualizar(string? name)
        {
            PropertyChanged?.Invoke(this, new(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
