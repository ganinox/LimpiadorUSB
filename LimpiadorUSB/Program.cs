using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LimpiadorUSB
{
    using System;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Eliminacion de Virus";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------- Eliminador de Virus  -------------");
            Console.WriteLine("----------------------------------------------");

            DriveInfo[] drives = DriveInfo.GetDrives();
            List<DriveInfo> removableDrives = new List<DriveInfo>();
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    removableDrives.Add(drive);
                }
            }

            if (removableDrives.Count == 0)
            {
                Console.WriteLine("No se ha encontrado una unidad de almacenamiento extraible.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Se han encontrado las siguientes unidades de almacenamiento extraibles:");
            for (int i = 0; i < removableDrives.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, removableDrives[i].Name);
            }

            Console.Write("Seleccione una unidad: ");
            int selectedDriveIndex;
            while (!int.TryParse(Console.ReadLine(), out selectedDriveIndex) || selectedDriveIndex < 1 || selectedDriveIndex > removableDrives.Count)
            {
                Console.WriteLine("El número ingresado no corresponde a una unidad de almacenamiento extraíble.");
                Console.WriteLine("Por favor, seleccione una unidad válida:");
            }
            selectedDriveIndex--;

            DriveInfo selectedDrive = removableDrives[selectedDriveIndex];
            string unidadSeleccionada = selectedDrive.RootDirectory.FullName;

            Console.WriteLine("Ha seleccionado la unidad {0}.", unidadSeleccionada);
            // Llamada a la funcion de recuperacion de archivos ocultos
            //RecuperarArchivos(unidadSeleccionada);
            MoverArchivosACarpetaVacia(unidadSeleccionada);
            // Llamada a la función EliminarVirus
            EliminarVirus(unidadSeleccionada);

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Presione cualquier tecla para salir");
            Console.ReadKey();
        }

        static void EliminarVirus(string unidadSeleccionada)
        {

            Console.WriteLine("Eliminando Accesos Directos....");
            int numAccessosDirectos = 0;
            DirectoryInfo dir = new DirectoryInfo(unidadSeleccionada);
            foreach (FileInfo file in dir.GetFiles("*.lnk", SearchOption.AllDirectories))
            {
                file.Delete();
                numAccessosDirectos++;
                Console.WriteLine("Acceso Directo Eliminado: " + file.FullName);
            }
            if (numAccessosDirectos > 0)
            {
                Console.WriteLine("Se han eliminado " + numAccessosDirectos + " accesos directos maliciosos.");
            }
            else
            {
                Console.WriteLine("No se han encontrado accesos directos maliciosos.");
            }
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Eliminando Infeccion...");
            int numArchivosMaliciosos = 0;
            if (File.Exists("autorun.inf"))
            {
                File.Delete("autorun.inf");
                numArchivosMaliciosos++;
            }
            if (File.Exists("desktop.ini"))
            {
                File.Delete("desktop.ini");
                numArchivosMaliciosos++;
            }
            if (File.Exists("streamer.exe"))
            {
                File.Delete("streamer.exe");
                numArchivosMaliciosos++;
            }
            Console.WriteLine("----------------------------------------------");
            if (numArchivosMaliciosos > 0)
            {
                Console.WriteLine("Se han eliminado " + numArchivosMaliciosos + " archivos maliciosos.");
            }
            else
            {
                Console.WriteLine("No se han encontrado archivos maliciosos.");
            }
        }
       


        public static void MoverArchivosACarpetaVacia(string unidadSeleccionada)
        {
            Console.WriteLine("Recuperando atributos de carpetas...");
            DirectoryInfo dir = new DirectoryInfo(unidadSeleccionada);
            foreach (FileInfo file in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                file.Attributes &= ~(FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.System);
            }
            Console.WriteLine("Recuperacion Exitosa!");
            Console.WriteLine("----------------------------------------------");
            int numArchivosMovidos = 0;
            int numArchivosCarpetaVacia = 0;
            string ruta = unidadSeleccionada;
            string carpetaVacia = "Alcatraz";



            //Probar si existe una carpeta vacia en el entorno
            string[] carpetas = Directory.GetDirectories(ruta, "*", SearchOption.TopDirectoryOnly);
            bool ExisteCarpetaVacia = false;
            Console.WriteLine("Verificando si existe una carpeta con contenido oculto.....");
            foreach (string carpeta in carpetas)
            {
                string nombreCarpeta = Path.GetFileName(carpeta);
                
                if (nombreCarpeta.Equals(carpetaVacia))
                {
                    ExisteCarpetaVacia = true;
                }
            }
            if (ExisteCarpetaVacia == true)
            {
                // Obtenemos los archivos y carpetas dentro de la carpeta vacía
                string[] archivosYCarpetas = Directory.GetFileSystemEntries(ruta + carpetaVacia, "*", SearchOption.TopDirectoryOnly);

                foreach (string archivoOCarpeta in archivosYCarpetas)
                {
                    // Obtenemos el nombre del archivo o carpeta
                    string nombreArchivoOCarpeta = Path.GetFileName(archivoOCarpeta);

                    // Creamos la ruta de la nueva ubicación del archivo o carpeta
                    string nuevaRuta = ruta + nombreArchivoOCarpeta;

                    // Si es un archivo, lo movemos a la nueva ubicación
                    if (File.Exists(archivoOCarpeta))
                    {
                        File.Move(archivoOCarpeta, nuevaRuta);
                        numArchivosMovidos++;
                    }
                    // Si es una carpeta, la movemos a la nueva ubicación (y todos sus archivos y carpetas internas)
                    else if (Directory.Exists(archivoOCarpeta))
                    {
                        Directory.Move(archivoOCarpeta, nuevaRuta);
                        numArchivosMovidos++;
                    }
                }
                Console.WriteLine("Se han movido " + numArchivosMovidos + " archivos a la raíz de la unidad " + unidadSeleccionada);
                Console.WriteLine("La carpeta vacía de la unidad " + unidadSeleccionada + " contenía " + numArchivosCarpetaVacia + " archivos");

                // Verificamos si la carpeta vacía no contiene archivos ni carpetas
                if (Directory.GetFiles(ruta + carpetaVacia).Length == 0 && Directory.GetDirectories(ruta + carpetaVacia).Length == 0)
                {
                    // Eliminamos la carpeta vacía
                    Directory.Delete(ruta + carpetaVacia);

                    Console.WriteLine("Se ha eliminado la carpeta vacía " + carpetaVacia);
                }

                Console.WriteLine("----------------------------------------------");
            }
            else
            {
                Console.WriteLine("No existe una carpeta vacía en la unidad " + unidadSeleccionada);
                Console.WriteLine("----------------------------------------------");
            }
            














            

        }
    }
}
