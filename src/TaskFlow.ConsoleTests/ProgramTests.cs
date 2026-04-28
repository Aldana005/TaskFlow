
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TaskFlow.Utils.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void Main_OtherKey_ExitsImmediately()
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;
            try
            {
                using var writer = new StringWriter();
                Console.SetOut(writer);

                using var sr = new StringReader("x\n");
                Console.SetIn(sr);

                Program.Main(Array.Empty<string>());

                var output = writer.ToString();
                StringAssert.Contains(output, "Saliendo");
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }

        [TestMethod]
        public void Main_SelectList_ShowsNoTasksInfo()
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;
            try
            {
                using var writer = new StringWriter();
                Console.SetOut(writer);

                // "2" selecciona listar tareas. Luego damos Enter para el filtro (mostrar todas).
                // Tras procesar la opción, Main volverá a leer y, al no haber más entradas, saldrá.
                using var sr = new StringReader("2\n\n");
                Console.SetIn(sr);

                Program.Main(Array.Empty<string>());

                var output = writer.ToString();
                StringAssert.Contains(output, "No hay tareas");
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }

        [TestMethod]
        public void Main_SelectDelete_InvalidInput_ShowsError()
        {
            var originalIn = Console.In;
            var originalOut = Console.Out;
            try
            {
                using var writer = new StringWriter();
                Console.SetOut(writer);

                // "5" selecciona eliminar. Luego enviamos "abc" como ID inválido.
                using var sr = new StringReader("5\nabc\n");
                Console.SetIn(sr);

                Program.Main(Array.Empty<string>());

                var output = writer.ToString();
                StringAssert.Contains(output, "Entrada no válida");
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }
    }
}