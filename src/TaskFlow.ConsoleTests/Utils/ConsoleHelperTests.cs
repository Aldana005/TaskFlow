using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskFlow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Services;

namespace TaskFlow.Utils.Tests
{
    [TestClass()]
    public class ConsoleHelperTests
    {
        private class MockTaskService : TaskService
        {
            public int LastId;
            public string LastResponsible = string.Empty;
            public bool UpdateResult = true;

            public new bool UpdateTaskResponsible(int id, string responsible)
            {
                LastId = id;
                LastResponsible = responsible;
                return UpdateResult;
            }
        }

        [TestMethod]
        public void PromptUpdateResponsible_IdNoValido_MuestraError()
        {
            var service = new MockTaskService();
            var input = new StringReader("abc\n");
            var output = new StringWriter();
            Console.SetIn(input);
            Console.SetOut(output);

            ConsoleHelper.PromptUpdateResponsible(service);

            StringAssert.Contains(output.ToString(), "Debe ingresar un número de ID válido");
        }

        [TestMethod]
        public void PromptUpdateResponsible_ResponsableVacio_MuestraError()
        {
            var service = new MockTaskService();
            var input = new StringReader("1\n   \n");
            var output = new StringWriter();
            Console.SetIn(input);
            Console.SetOut(output);

            ConsoleHelper.PromptUpdateResponsible(service);

            StringAssert.Contains(output.ToString(), "El nombre del responsable no puede estar vacío");
        }

        
        [TestMethod]
        public void PromptUpdateResponsible_TareaNoEncontrada_MuestraError()
        {
            var service = new MockTaskService { UpdateResult = false };
            var input = new StringReader("3\nAna\n");
            var output = new StringWriter();
            Console.SetIn(input);
            Console.SetOut(output);

            ConsoleHelper.PromptUpdateResponsible(service);

            StringAssert.Contains(output.ToString(), "No se encontró ninguna tarea con el ID #3");
        }
        


        [TestMethod]
        public void PromptUpdateResponsible_ResponsableSoloEspacios_MuestraError()
        {
            var service = new MockTaskService();
            var input = new StringReader("10\n    \n");
            var output = new StringWriter();
            Console.SetIn(input);
            Console.SetOut(output);

            ConsoleHelper.PromptUpdateResponsible(service);

            StringAssert.Contains(output.ToString(), "El nombre del responsable no puede estar vacío");
        }

        [TestMethod]
        public void PromptUpdateResponsible_IdVacio_MuestraError()
        {
            var service = new MockTaskService();
            var input = new StringReader("\n");
            var output = new StringWriter();
            Console.SetIn(input);
            Console.SetOut(output);

            ConsoleHelper.PromptUpdateResponsible(service);

            StringAssert.Contains(output.ToString(), "Debe ingresar un número de ID válido");
        }

        [TestMethod]
        public void PromptUpdateResponsible_ResponsableNulo_MuestraError()
        {
            var service = new MockTaskService();
            var input = new StringReader("1\n\n");
            var output = new StringWriter();
            Console.SetIn(input);
            Console.SetOut(output);

            ConsoleHelper.PromptUpdateResponsible(service);

            StringAssert.Contains(output.ToString(), "El nombre del responsable no puede estar vacío");
        }

    }
}