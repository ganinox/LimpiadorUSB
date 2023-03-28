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

            // Llamada a la función EliminarVirus
            EliminarVirus(unidadSeleccionada);

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Presione cualquier tecla para salir");
            Console.ReadKey();
        }

        static void EliminarVirus(string unidadSeleccionada)
        {
            Console.WriteLine("Recuperando atributos de carpetas...");
            DirectoryInfo dir = new DirectoryInfo(unidadSeleccionada);
            foreach (FileInfo file in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                file.Attributes &= ~(FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.System);
            }
            Console.WriteLine("Recuperacion Exitosa!");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Eliminando Accesos Directos....");
            int numAccessosDirectos = 0;
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
       
        
    }
}
