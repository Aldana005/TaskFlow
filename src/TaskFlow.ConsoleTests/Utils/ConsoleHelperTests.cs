using Microsoft.VisualStudio.TestTools.UnitTesting;
<<<<<<< HEAD
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TaskFlow.Services;
using TaskFlow.Utils;
using System.Collections.Generic;
=======
using TaskFlow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Services;
>>>>>>> 192dfe93886e7660d0fbb0c6c3b839b1c6f05943

namespace TaskFlow.Utils.Tests
{
    [TestClass()]
    public class ConsoleHelperTests
    {
<<<<<<< HEAD
        private string CreateTempFolder(out string filePath)
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), "TaskFlowTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            filePath = Path.Combine(tempFolder, "tasks.json");
            return tempFolder;
        }

        private void Cleanup(string folder, string file)
        {
            try
            {
                if (File.Exists(file)) File.Delete(file);
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
            catch
            {
                // evitar fallos en CI por cleanup
=======
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
>>>>>>> 192dfe93886e7660d0fbb0c6c3b839b1c6f05943
            }
        }

        [TestMethod]
<<<<<<< HEAD
        public void PromptDeleteTask_InvalidInput_ShowsError()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    using var sr = new StringReader("no-es-numero" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("Entrada no válida"), "Debe informar que la entrada no es válida.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void PromptDeleteTask_IdNotFound_ShowsError()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    using var sr = new StringReader("12345" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("No se encontró la tarea"), "Debe informar que no encontró la tarea con ese ID.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void PromptDeleteTask_ConfirmDeletion_DeletesTaskAndShowsSuccess()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Tarea A", "Desc", "R");
                var id = svc.GetTasks().First().Id;

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    // primera línea: id, segunda línea: confirmar con "s"
                    using var sr = new StringReader(id + Environment.NewLine + "s" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("Tarea eliminada correctamente") || output.Contains("[Éxito]"), "Debe mostrar mensaje de éxito.");
                    Assert.IsNull(svc.GetTaskById(id), "La tarea debe haber sido eliminada del servicio.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void PromptDeleteTask_CancelDeletion_DoesNotDeleteAndShowsCancelInfo()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Tarea B", "Desc", "R2");
                var id = svc.GetTasks().First().Id;

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    // id luego "n" para cancelar
                    using var sr = new StringReader(id + Environment.NewLine + "n" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("Operación cancelada"), "Debe indicar que la operación fue cancelada.");
                    Assert.IsNotNull(svc.GetTaskById(id), "La tarea no debe haberse eliminado.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        // ----------------- Pruebas adicionales para aumentar cobertura -----------------

        [TestMethod]
        public void PromptDeleteTask_ConfirmWithUppercaseS_DeletesTask()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("UpperCase", "Desc", "R");
                var id = svc.GetTasks().First().Id;

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    // usar "S" en mayúscula para confirmar (se hace ToLower() en el método)
                    using var sr = new StringReader(id + Environment.NewLine + "S" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("Tarea eliminada correctamente") || output.Contains("[Éxito]"), "La confirmación en mayúscula debe ser aceptada.");
                    Assert.IsNull(svc.GetTaskById(id), "La tarea debe quedar eliminada.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void PromptDeleteTask_ConfirmationOtherThanS_IsTreatedAsCancel()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Otra", "Desc", "R3");
                var id = svc.GetTasks().First().Id;

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    // confirmar con "si" (no es exactamente "s") => debe cancelar
                    using var sr = new StringReader(id + Environment.NewLine + "si" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("Operación cancelada"), "Cualquier entrada distinta de 's' debe cancelar la operación.");
                    Assert.IsNotNull(svc.GetTaskById(id), "La tarea no debe eliminarse.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void PromptDeleteTask_InputWithWhitespace_ParsesAndDeletes()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Whitespace", "Desc", "R4");
                var id = svc.GetTasks().First().Id;

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    // input con espacios alrededor del id, debe parsear correctamente
                    using var sr = new StringReader("  " + id + "  " + Environment.NewLine + "s" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("Tarea eliminada correctamente") || output.Contains("[Éxito]"), "El parser debe manejar espacios alrededor del ID.");
                    Assert.IsNull(svc.GetTaskById(id), "La tarea debe haber sido eliminada.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void PromptDeleteTask_TaskWithUpdatedAt_PrintsUpdatedDateInsteadOfNA()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("WithUpdate", "Desc", "R5");
                var id = svc.GetTasks().First().Id;

                // Forzar UpdatedAt a través de UpdateTaskStatus (mecanismo existente)
                svc.UpdateTaskStatus(id, TaskStatus.EnProgreso);

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    using var sr = new StringReader(id + Environment.NewLine + "n" + Environment.NewLine); // cancelar tras visualización
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);

                    var output = writer.ToString();
                    // Si UpdatedAt tiene valor, la vista NO debe mostrar "N/A" en la sección Modificada
                    Assert.IsFalse(output.Contains("Modificada: N/A"), "Cuando UpdatedAt existe, no debe mostrarse 'N/A'.");
                    Assert.IsTrue(output.Contains("Modificada:"), "Debe imprimirse la etiqueta de fecha de modificación.");
                    // Aseguramos que la tarea no se eliminó por usar "n"
                    Assert.IsNotNull(svc.GetTaskById(id), "La tarea no debe haberse eliminado por cancelar.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        // Nueva: comprueba que cuando UpdatedAt NO existe se vea "Modificada: N/A"
        [TestMethod]
        public void PromptDeleteTask_NoUpdatedAt_ShowsNA()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("SinUpdate", "Desc", "R7");
                var id = svc.GetTasks().First().Id;

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    using var sr = new StringReader(id + Environment.NewLine + "n" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("Modificada: N/A"), "Cuando UpdatedAt es null debe mostrarse 'Modificada: N/A'.");
                    Assert.IsTrue(output.Contains("Creada:"), "Debe mostrarse la fecha de creación.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        // Nueva: verifica que se imprime la línea separadora de 40 guiones
        [TestMethod]
        public void PromptDeleteTask_PrintsDividerLine()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("DividerTest", "Desc", "R8");
                var id = svc.GetTasks().First().Id;

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    using var sr = new StringReader(id + Environment.NewLine + "n" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains(new string('-', 40)), "Debe imprimirse la línea separadora de 40 guiones.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        // Nueva: llamadas secuenciales al prompt consumiendo múltiples líneas del mismo input
        [TestMethod]
        public void PromptDeleteTask_SequentialCalls_DeletesMultipleTasks()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Seq1", "D1", "R1");
                svc.CreateTask("Seq2", "D2", "R2");

                var tasks = svc.GetTasks().ToList();
                var id1 = tasks[0].Id;
                var id2 = tasks[1].Id;

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    // proporcionamos entradas para dos prompts consecutivos
                    using var sr = new StringReader(
                        id1 + Environment.NewLine + "s" + Environment.NewLine +
                        id2 + Environment.NewLine + "s" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);
                    ConsoleHelper.PromptDeleteTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("Tarea eliminada correctamente") || Regex.IsMatch(output, @"\[Éxito\]"), "Debe mostrar mensajes de éxito para ambas eliminaciones.");
                    Assert.IsNull(svc.GetTaskById(id1), "La primera tarea debe eliminarse.");
                    Assert.IsNull(svc.GetTaskById(id2), "La segunda tarea debe eliminarse.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        // Nueva: primer intento inválido y luego intento válido, usando el mismo stream de entrada
        [TestMethod]
        public void PromptDeleteTask_InvalidThenValid_AttemptsHandleBoth()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("FlowTest", "D", "R");
                var id = svc.GetTasks().First().Id;

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    // primer Prompt: entrada inválida -> "Entrada no válida"
                    // segundo Prompt: id válido + confirmar -> elimina
                    using var sr = new StringReader(
                        "abc" + Environment.NewLine +
                        id + Environment.NewLine + "s" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc); // consume "abc"
                    ConsoleHelper.PromptDeleteTask(svc); // consume id + s

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("Entrada no válida"), "Primer intento inválido debe informar error.");
                    Assert.IsTrue(output.Contains("Tarea eliminada correctamente") || output.Contains("[Éxito]"), "Segundo intento debe eliminar la tarea.");
                    Assert.IsNull(svc.GetTaskById(id), "La tarea debe quedar eliminada después del segundo intento.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }
=======
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

>>>>>>> 192dfe93886e7660d0fbb0c6c3b839b1c6f05943
    }
}