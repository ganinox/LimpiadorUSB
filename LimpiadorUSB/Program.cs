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
            Console.WriteLine("Recuperando atributos de carpetas...");
            DirectoryInfo dir = new DirectoryInfo(".");
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
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Presione cualquier tecla para salir");
            Console.ReadKey();
        }
    }

}
