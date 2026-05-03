using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Services;
using TaskFlow.Utils;

namespace TaskFlow.Utils.Tests
{
    [TestClass()]
    public class ConsoleHelperTests
    {
        // ==========================================
        // HELPERS DEL EQUIPO A (Archivos Temporales)
        // ==========================================
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
            }
        }
        // ==========================================
        // TESTS DE CREAR TAREAS(PromptCreateTask)
        // ==========================================
        [TestMethod]
        public void PromptCreateTask_ValidInput_CreatesTaskAndShowsSuccess()
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

                    // Título, descripción, responsable
                    using var sr = new StringReader("Mi Tarea\nDescripción de prueba\nResponsableX\n");
                    Console.SetIn(sr);

                    ConsoleHelper.PromptCreateTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("[Éxito]") || output.Contains("creada"), "Debe mostrar mensaje de éxito al crear la tarea.");

                    var tasks = svc.GetTasks();
                    Assert.IsTrue(tasks.Any(t => t.Title == "Mi Tarea" && t.Responsible == "ResponsableX"), "La tarea debe quedar registrada en el servicio.");
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
        public void PromptCreateTask_EmptyTitle_RepeatsPromptUntilValid()
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

                    // Primeros dos inputs de título vacíos, luego título válido, descripción y responsable
                    using var sr = new StringReader("\n   \nTítuloFinal\nDesc\nResp\n");
                    Console.SetIn(sr);

                    ConsoleHelper.PromptCreateTask(svc);

                    var output = writer.ToString();
                    // Debe haber solicitado el título reiteradas veces (comprobamos que se haya creado la tarea correcta)
                    var tasks = svc.GetTasks();
                    Assert.IsTrue(tasks.Any(t => t.Title == "TítuloFinal"), "Después de reintentos, la tarea con título válido debe crearse.");
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
        public void PromptCreateTask_ServiceThrows_ShowsErrorMessage()
        {
            // Provocamos una excepción en SaveTasks pasando customFile igual al folder (escribirá en una "ruta" que es un directorio)
            var folder = CreateTempFolder(out var file);
            try
            {
                // Aquí pasamos customFile igual al folder para forzar que File.WriteAllText intente escribir en un directorio y lance.
                var svc = new TaskService(folder, folder);

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);

                    using var sr = new StringReader("TituloError\nDesc\nResp\n");
                    Console.SetIn(sr);

                    ConsoleHelper.PromptCreateTask(svc);

                    var output = writer.ToString();
                    Assert.IsTrue(output.Contains("[Error]") || output.Contains("Error"), "Si el servicio lanza, debe mostrarse un mensaje de error.");
                }
                finally
                {
                    Console.SetIn(originalIn);
                    Console.SetOut(originalOut);
                }
            }
            finally { Cleanup(folder, file); }
        }

        // ==========================================
        // HELPERS DEL EQUIPO B (Mocking)
        // ==========================================
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


        // ==========================================
        // TESTS DE ELIMINAR TAREA (PromptDeleteTask)
        // ==========================================

        [TestMethod]
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

                svc.UpdateTaskStatus(id, TaskStatus.EnProgreso);
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
                    Assert.IsFalse(output.Contains("Modificada: N/A"), "Cuando UpdatedAt existe, no debe mostrarse 'N/A'.");
                    Assert.IsTrue(output.Contains("Modificada:"), "Debe imprimirse la etiqueta de fecha de modificación.");
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

                    using var sr = new StringReader(
                        "abc" + Environment.NewLine +
                        id + Environment.NewLine + "s" + Environment.NewLine);
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);
                    ConsoleHelper.PromptDeleteTask(svc);

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

        // ==================================================
        // TESTS DE ACTUALIZAR RESPONSABLE (PromptUpdateResp)
        // ==================================================

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
        [TestMethod]
        public void PrintTaskList_Empty_ShowsInfoMessage()
        {
            var originalOut = Console.Out;
            try
            {
                using var writer = new StringWriter();
                Console.SetOut(writer);

                ConsoleHelper.PrintTaskList(new List<TaskItem>());
                StringAssert.Contains(writer.ToString(), "No hay tareas");
            }
            finally { Console.SetOut(originalOut); }
        }

        [TestMethod]
        public void PromptUpdateStatus_NonNumericId_ShowsError()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);
                    using var sr = new StringReader("abc\n");
                    Console.SetIn(sr);

                    ConsoleHelper.PromptUpdateStatus(svc);
                    StringAssert.Contains(writer.ToString(), "Debe ingresar un número de ID válido");
                }
                finally { Console.SetIn(originalIn); Console.SetOut(originalOut); }
            }
            finally { Cleanup(folder,file); }
        }

        [TestMethod]
        public void PromptUpdateResponsible_EmptyResponsible_ShowsError()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("T", null, "R");

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);
                    // first line: id, second line: empty responsible
                    using var sr = new StringReader("1\n   \n");
                    Console.SetIn(sr);

                    ConsoleHelper.PromptUpdateResponsible(svc);
                    StringAssert.Contains(writer.ToString(), "El nombre del responsable no puede estar vacío");
                }
                finally { Console.SetIn(originalIn); Console.SetOut(originalOut); }
            }
            finally { Cleanup(folder,file); }
        }

        [TestMethod]
        public void PromptDeleteTask_Cancel_ShowsCancelled()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("T", null, "R");

                var originalIn = Console.In;
                var originalOut = Console.Out;
                try
                {
                    using var writer = new StringWriter();
                    Console.SetOut(writer);
                    // id, then 'n' to cancel
                    using var sr = new StringReader("1\nn\n");
                    Console.SetIn(sr);

                    ConsoleHelper.PromptDeleteTask(svc);
                    StringAssert.Contains(writer.ToString(), "Operación cancelada");
                    // ensure task still exists
                    Assert.IsNotNull(svc.GetTaskById(1));
                }
                finally { Console.SetIn(originalIn); Console.SetOut(originalOut); }
            }
            finally { Cleanup(folder,file); }
        }

        private (string folder, string file) CreateTempPaths()
        {
            var folder = Path.Combine(Path.GetTempPath(), "TaskFlowTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(folder);
            var file = Path.Combine(folder, "tasks.json");
            return (folder, file);
        }

        [TestMethod]
        public void PromptLogin_CaminoFeliz_DatosCorrectos_RetornaTrue()
        {
            // Arrange
            var authService = new AuthService();

            // Simulamos lo que el usuario tipea separado por \n (que equivale a apretar Enter)
            // Primero "Aldana" [Enter], luego "Sánchez" [Enter]
            var entradaSimulada = new StringReader("Aldana\nSánchez\n");
            Console.SetIn(entradaSimulada);

            // Redirigimos la salida (WriteLine) a la nada misma para que no ensucie el test
            Console.SetOut(new StringWriter());

            // Act
            bool result = ConsoleHelper.PromptLogin(authService);

            // Assert
            Assert.IsTrue(result, "Debería retornar true cuando las credenciales son correctas.");
        }

        [TestMethod]
        public void PromptLogin_CaminoTriste_DatosIncorrectosEligeSalir_RetornaFalse()
        {
            // Arrange
            var authService = new AuthService();

            // Simulamos: "Juan" [Enter], "Falso" [Enter], ¿Intentar de nuevo? "N" [Enter]
            var entradaSimulada = new StringReader("Juan\nFalso\nN\n");
            Console.SetIn(entradaSimulada);
            Console.SetOut(new StringWriter());

            // Act
            bool result = ConsoleHelper.PromptLogin(authService);

            // Assert
            Assert.IsFalse(result, "Debería retornar false cuando falla y el usuario no quiere reintentar.");
        }

        [TestMethod]
        public void PromptLogin_CaminoRecuperacion_FallaYLuegoAcierta_RetornaTrue()
        {
            // Arrange
            var authService = new AuthService();

            // Simulamos: Falla primero ("Usuario\nMal\n"), dice que SÍ quiere intentar ("S\n"), y acierta ("Aldana\nSánchez\n")
            var entradaSimulada = new StringReader("Usuario\nMal\nS\nAldana\nSánchez\n");
            Console.SetIn(entradaSimulada);
            Console.SetOut(new StringWriter());

            // Act
            bool result = ConsoleHelper.PromptLogin(authService);

            // Assert
            Assert.IsTrue(result, "Debería retornar true si falla la primera vez pero acierta en el reintento.");
        }
    }
}
