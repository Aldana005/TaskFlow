using Microsoft.VisualStudio.TestTools.UnitTesting;
<<<<<<< HEAD
<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using TaskFlow.Services;
=======
>>>>>>> 6934155f4dfdf3b63b06feb9b170032963f8b033
=======
using TaskFlow.Services;
>>>>>>> 192dfe93886e7660d0fbb0c6c3b839b1c6f05943
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
<<<<<<< HEAD
using System.Text.Json;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using TaskFlow.Services;
=======
using System.Threading.Tasks;
>>>>>>> 6934155f4dfdf3b63b06feb9b170032963f8b033
>>>>>>> 192dfe93886e7660d0fbb0c6c3b839b1c6f05943

namespace TaskFlow.Services.Tests
{
    [TestClass()]
    public class TaskServiceTests
    {
<<<<<<< HEAD
<<<<<<< HEAD
        // --- MÉTODOS DE APOYO (Helpers) ---

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
                // No lanzar en cleanup para evitar falsos negativos en entornos de CI
            }
        }

        // --- TESTS DE ELIMINACIÓN (DeleteTask) ---

        [TestMethod]
        public void DeleteTask_TaskExists_ReturnsTrueAndRemovesTask()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Tarea 1", "Desc 1", "A");
                svc.CreateTask("Tarea 2", "Desc 2", "B");

                var before = svc.GetTasks();
                var idToDelete = before.First().Id;

                var result = svc.DeleteTask(idToDelete);

                Assert.IsTrue(result, "DeleteTask debe devolver true para una tarea existente.");
                Assert.AreEqual(1, svc.GetTasks().Count, "Debe disminuir el número de tareas.");
                Assert.IsNull(svc.GetTaskById(idToDelete), "La tarea borrada no debe encontrarse por id.");

                // Verificación de persistencia básica
                var content = File.ReadAllText(file);
                Assert.IsTrue(content.Contains("\"Id\""), "El JSON resultante debe ser válido.");
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void DeleteTask_TaskDoesNotExist_ReturnsFalseAndListUnchanged()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Solo tarea", "Desc", "A");

                var result = svc.DeleteTask(9999); // ID inexistente

                Assert.IsFalse(result);
                Assert.AreEqual(1, svc.GetTasks().Count, "La lista no debe cambiar.");
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void DeleteTask_DeleteSameTaskTwice_FirstTrueThenFalse()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Tarea X", "Desc", "X");
                var id = svc.GetTasks().First().Id;

                Assert.IsTrue(svc.DeleteTask(id));
                Assert.IsFalse(svc.DeleteTask(id), "La segunda eliminación debe fallar.");
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void DeleteTask_EmptyService_ReturnsFalse()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                Assert.IsFalse(svc.DeleteTask(1));
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void DeleteTask_RemovesCorrectTaskAmongMany()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("T1", "D1", "A");
                svc.CreateTask("T2", "D2", "B");
                svc.CreateTask("T3", "D3", "C");

                var tasks = svc.GetTasks().ToList();
                var middleId = tasks[1].Id;

                Assert.IsTrue(svc.DeleteTask(middleId));
                Assert.IsNull(svc.GetTaskById(middleId), "Debe eliminar la tarea específica, no por posición.");
                Assert.AreEqual(2, svc.GetTasks().Count);
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void DeleteTask_PersistsRemovalInJson()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Persist1", "D1", "A");
                var idToDelete = svc.GetTasks().First().Id;

                svc.DeleteTask(idToDelete);

                var content = File.ReadAllText(file);
                Assert.IsFalse(Regex.IsMatch(content, $"\"Id\"\\s*:\\s*{idToDelete}"), "El ID eliminado no debe figurar en el JSON.");
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void DeleteTask_LoadTasksMalformedJson_DoesNotCrashAndReturnsFalse()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                Directory.CreateDirectory(folder);
                File.WriteAllText(file, "esto no es un json");

                var svc = new TaskService(folder, file);
                Assert.IsFalse(svc.DeleteTask(1), "Debe manejar el error de carga y fallar el borrado suavemente.");
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void DeleteTask_NegativeId_ReturnsFalse()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("TNeg", "Desc", "N");
                Assert.IsFalse(svc.DeleteTask(-5));
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void DeleteTask_DeleteAllSequentially_AllRemoved()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var svc = new TaskService(folder, file);
                for (int i = 0; i < 5; i++) svc.CreateTask($"T{i}", $"D{i}", $"R{i}");

                var ids = svc.GetTasks().Select(t => t.Id).ToList();
                foreach (var id in ids)
                {
                    Assert.IsTrue(svc.DeleteTask(id));
                }

                Assert.AreEqual(0, svc.GetTasks().Count);
            }
            finally { Cleanup(folder, file); }
        }
        [TestMethod]
        public void DeleteTask_LoadsFromJsonFileAndDeletes_IdRemovedFromLoadedData()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                // Preparar JSON manualmente con varios TaskItem (se evita llamar a CreateTask)
                var list = new List<TaskItem>
                {
                    new TaskItem { Id = 101, Title = "A", Description = "a", Responsible = "R", Status = TaskStatus.Pendiente },
                    new TaskItem { Id = 202, Title = "B", Description = "b", Responsible = "R", Status = TaskStatus.Pendiente },
                    new TaskItem { Id = 303, Title = "C", Description = "c", Responsible = "R", Status = TaskStatus.Pendiente }
                };
                File.WriteAllText(file, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file); // carga desde archivo
                Assert.IsTrue(svc.DeleteTask(202));
                Assert.IsFalse(File.ReadAllText(file).Contains("\"Id\": 202"));
                Assert.IsNull(svc.GetTaskById(202));
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void DeleteTask_FileReadOnly_ThrowsUnauthorizedAccessException()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var list = new List<TaskItem>
                {
                    new TaskItem { Id = 1, Title = "X", Description = "x", Responsible = "R", Status = TaskStatus.Pendiente }
                };
                File.WriteAllText(file, JsonSerializer.Serialize(list));
                // Marcar como solo lectura para provocar fallo en SaveTasks durante DeleteTask
                File.SetAttributes(file, FileAttributes.ReadOnly);

                var svc = new TaskService(folder, file);
                Assert.ThrowsException<UnauthorizedAccessException>(() => svc.DeleteTask(1));
            }
            finally
            {
                // Restaurar atributos para permitir borrado
                try { File.SetAttributes(file, FileAttributes.Normal); } catch { }
                Cleanup(folder, file);
=======
        private static (string folder, string file) CreateTempPaths()
        {
            string folder = Path.Combine(Path.GetTempPath(), "TaskFlowTests", Guid.NewGuid().ToString());
            string file = Path.Combine(folder, "tasks.json");
            return (folder, file);
        }

        [TestMethod]
        public void GetTaskById_NoTasks_ReturnsNull()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                var result = svc.GetTaskById(1);
                Assert.IsNull(result);
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void GetTaskById_SingleTask_ReturnsThatTask()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("T1", "D1", "R1");

                var t = svc.GetTaskById(1);
                Assert.IsNotNull(t);
                Assert.AreEqual(1, t.Id);
                Assert.AreEqual("T1", t.Title);
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void GetTaskById_MultipleTasks_ReturnsCorrectById_First()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("A", "a", "ra");
                svc.CreateTask("B", "b", "rb");
                svc.CreateTask("C", "c", "rc");

                var t = svc.GetTaskById(1);
                Assert.IsNotNull(t);
                Assert.AreEqual("A", t.Title);
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void GetTaskById_MultipleTasks_ReturnsCorrectById_Last()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("A", "a", "ra");
                svc.CreateTask("B", "b", "rb");
                svc.CreateTask("C", "c", "rc");

                var t = svc.GetTaskById(3);
                Assert.IsNotNull(t);
                Assert.AreEqual("C", t.Title);
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void GetTaskById_NonExistingId_ReturnsNull()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Only", "only", "r");

                var t = svc.GetTaskById(42);
                Assert.IsNull(t);
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void GetTaskById_ZeroOrNegative_ReturnsNull()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("T", "D", "R");

                Assert.IsNull(svc.GetTaskById(0));
                Assert.IsNull(svc.GetTaskById(-5));
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void GetTaskById_ReturnsReference_ModifyingReturnedAffectsService()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Orig", "Desc", "Resp");

                var t = svc.GetTaskById(1);
                Assert.IsNotNull(t);
                t.Responsible = "Changed";

                var t2 = svc.GetTaskById(1);
                Assert.AreEqual("Changed", t2.Responsible, "La referencia devuelta debe ser la misma instancia almacenada internamente.");
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void GetTaskById_LoadedFile_ReturnsExpected()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                Directory.CreateDirectory(folder);

                var tasks = new List<TaskItem>
                {
                    new TaskItem { Id = 100, Title = "T100", Description = "d1", Responsible = "r1", Status = TaskStatus.Pendiente, CreatedAt = DateTime.UtcNow },
                    new TaskItem { Id = 200, Title = "T200", Description = "d2", Responsible = "r2", Status = TaskStatus.EnProgreso, CreatedAt = DateTime.UtcNow }
                };

                File.WriteAllText(file, JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file);
                var t = svc.GetTaskById(200);

                Assert.IsNotNull(t);
                Assert.AreEqual(200, t.Id);
                Assert.AreEqual("T200", t.Title);
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
>>>>>>> 6934155f4dfdf3b63b06feb9b170032963f8b033
            }
        }

        [TestMethod]
<<<<<<< HEAD
        public void DeleteTask_DeleteNonexistentIdAfterLoading_ReturnsFalse()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                var list = new List<TaskItem>
                {
                    new TaskItem { Id = 10, Title = "X", Description = "x", Responsible = "R", Status = TaskStatus.Pendiente },
                    new TaskItem { Id = 20, Title = "Y", Description = "y", Responsible = "R", Status = TaskStatus.Pendiente }
                };
                File.WriteAllText(file, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file);
                Assert.IsFalse(svc.DeleteTask(9999), "Eliminar un id que no existe en datos cargados debe devolver false.");
                // Asegurarse de que los ids originales permanecen
                Assert.IsNotNull(svc.GetTaskById(10));
                Assert.IsNotNull(svc.GetTaskById(20));
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void DeleteTask_CreatesConfiguredFolder_WhenFolderMissingOnSave()
        {
            var tempBase = Path.Combine(Path.GetTempPath(), "TaskFlowTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempBase);
            var fileInside = Path.Combine(tempBase, "tasks.json");
            // Crear datos iniciales en un archivo dentro de tempBase
            var list = new List<TaskItem>
            {
                new TaskItem { Id = 55, Title = "Z", Description = "z", Responsible = "R", Status = TaskStatus.Pendiente }
            };
            File.WriteAllText(fileInside, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

            // Pasar como carpeta configurada una ruta que NO existe
            var missingFolder = Path.Combine(tempBase, "willBeCreated");
            var svc = new TaskService(missingFolder, fileInside); // carga desde fileInside
            try
            {
                // Al eliminar, SaveTasks verificará _folderPath (missingFolder) y lo creará
                Assert.IsTrue(svc.DeleteTask(55));
                Assert.IsTrue(Directory.Exists(missingFolder), "SaveTasks debe crear la carpeta configurada si no existe.");
            }
            finally
            {
                Cleanup(tempBase, fileInside);
                try { if (Directory.Exists(missingFolder)) Directory.Delete(missingFolder, true); } catch { }
            }
        }
    
       [TestMethod]
        public void DeleteTask_DuplicateIds_RemovesOnlyFirstOccurrence()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                // Preparar JSON manualmente con IDs duplicados (mismo Id repetido)
                var list = new List<TaskItem>
                {
                    new TaskItem { Id = 7, Title = "A", Description = "a", Responsible = "R", Status = TaskStatus.Pendiente },
                    new TaskItem { Id = 7, Title = "B", Description = "b", Responsible = "R", Status = TaskStatus.Pendiente },
                    new TaskItem { Id = 8, Title = "C", Description = "c", Responsible = "R", Status = TaskStatus.Pendiente }
                };
                File.WriteAllText(file, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file); // carga duplicados
                // Al eliminar id 7, debe eliminar la primera ocurrencia encontrada
                Assert.IsTrue(svc.DeleteTask(7));

                var remainingJson = File.ReadAllText(file);
                // Debe quedar exactamente una ocurrencia del Id 7 en JSON
                var matches = Regex.Matches(remainingJson, "\"Id\"\\s*:\\s*7");
                Assert.AreEqual(1, matches.Count, "Debe quedar exactamente una entrada con Id 7 tras la eliminación.");
                // Y el otro id debe seguir presente
                Assert.IsNotNull(svc.GetTaskById(8));
            }
            finally { Cleanup(folder, file); }
        }

        [TestMethod]
        public void DeleteTask_FileLockedByAnotherProcess_ResultsInWriteFailure()
        {
            var folder = CreateTempFolder(out var file);
            FileStream lockStream = null;
            try
            {
                var list = new List<TaskItem>
                {
                    new TaskItem { Id = 42, Title = "LockMe", Description = "x", Responsible = "R", Status = TaskStatus.Pendiente }
                };
                File.WriteAllText(file, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file); // carga bien

                // Abrir el archivo con bloqueo exclusivo para provocar fallo en SaveTasks
                lockStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                bool threwExpected = false;
                try
                {
                    svc.DeleteTask(42);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    threwExpected = true; // Aceptamos cualquiera de las excepciones típicas de fallo de escritura
                }

                Assert.IsTrue(threwExpected, "La eliminación debe fallar con IOException o UnauthorizedAccessException cuando el archivo está bloqueado.");
            }
            finally
            {
                try { lockStream?.Dispose(); } catch { }
                Cleanup(folder, file);
=======
        public void GetTaskById_DuplicateIds_File_ReturnsFirstOccurrence()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                Directory.CreateDirectory(folder);

                var tasks = new List<TaskItem>
                {
                    new TaskItem { Id = 1, Title = "First", Description = "x", Responsible = "a", CreatedAt = DateTime.UtcNow },
                    new TaskItem { Id = 1, Title = "Second", Description = "y", Responsible = "b", CreatedAt = DateTime.UtcNow }
                };

                File.WriteAllText(file, JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file);
                var t = svc.GetTaskById(1);

                Assert.IsNotNull(t);
                Assert.AreEqual("First", t.Title, "Debe devolver la primera ocurrencia con el mismo Id.");
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void GetTaskById_MaxIntId_File_ReturnsTask()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                Directory.CreateDirectory(folder);

                var tasks = new List<TaskItem>
                {
                    new TaskItem { Id = int.MaxValue, Title = "MaxInt", Description = "mx", Responsible = "r", CreatedAt = DateTime.UtcNow }
                };

                File.WriteAllText(file, JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file);
                var t = svc.GetTaskById(int.MaxValue);

                Assert.IsNotNull(t);
                Assert.AreEqual("MaxInt", t.Title);
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
>>>>>>> 6934155f4dfdf3b63b06feb9b170032963f8b033
            }
        }

        [TestMethod]
<<<<<<< HEAD
        public void DeleteTask_DeleteZeroId_RemovesCorrectly()
        {
            var folder = CreateTempFolder(out var file);
            try
            {
                // Preparar JSON con id 0 explícito
                var list = new List<TaskItem>
                {
                    new TaskItem { Id = 0, Title = "Zero", Description = "zero", Responsible = "R", Status = TaskStatus.Pendiente },
                    new TaskItem { Id = 2, Title = "Two", Description = "two", Responsible = "R", Status = TaskStatus.Pendiente }
                };
                File.WriteAllText(file, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file);
                Assert.IsTrue(svc.DeleteTask(0), "Debe poder eliminar una tarea con Id 0.");
                Assert.IsNull(svc.GetTaskById(0));
                Assert.IsNotNull(svc.GetTaskById(2));
                Assert.IsFalse(File.ReadAllText(file).Contains("\"Id\": 0"), "El JSON persistido no debe contener el Id 0 eliminado.");
            }
            finally { Cleanup(folder, file); }
=======
        public void GetTaskById_ZeroAndNegativeIdsLoaded_ReturnsThem()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                Directory.CreateDirectory(folder);

                var tasks = new List<TaskItem>
                {
                    new TaskItem { Id = 0, Title = "Zero", Description = "z", Responsible = "rz", CreatedAt = DateTime.UtcNow },
                    new TaskItem { Id = -5, Title = "NegFive", Description = "n", Responsible = "rn", CreatedAt = DateTime.UtcNow }
                };

                File.WriteAllText(file, JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file);
                var t0 = svc.GetTaskById(0);
                var tn = svc.GetTaskById(-5);

                Assert.IsNotNull(t0);
                Assert.AreEqual("Zero", t0.Title);

                Assert.IsNotNull(tn);
                Assert.AreEqual("NegFive", tn.Title);
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        [TestMethod]
        public void GetTaskById_LargeList_File_ReturnsCorrectItem()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                Directory.CreateDirectory(folder);

                const int count = 500;
                var tasks = new List<TaskItem>(count);
                for (int i = 1; i <= count; i++)
                {
                    tasks.Add(new TaskItem { Id = i * 10, Title = $"T{i * 10}", CreatedAt = DateTime.UtcNow });
                }

                File.WriteAllText(file, JsonSerializer.Serialize(tasks));

                var svc = new TaskService(folder, file);
                int target = 2500; // exists because 250 * 10 = 2500 and count = 500
                var t = svc.GetTaskById(target);

                Assert.IsNotNull(t);
                Assert.AreEqual($"T{target}", t.Title);
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
>>>>>>> 6934155f4dfdf3b63b06feb9b170032963f8b033
=======
        private string testFolder = "test_data";
        private string testFile = "test_data/tasks_test.json";

        [TestInitialize]
        public void Setup()
        {
            if (Directory.Exists(testFolder))
                Directory.Delete(testFolder, true);
        }

        [TestMethod]
        public void UpdateTaskResponsible_TaskExists_UpdatesResponsibleAndReturnsTrue()
        {
            var service = new TaskService(testFolder, testFile);
            service.CreateTask("Tarea 1", "Desc", "Juan");
            var task = service.GetTasks()[0];
            var oldDate = task.UpdatedAt;

            var result = service.UpdateTaskResponsible(task.Id, "Maria");
            var updatedTask = service.GetTaskById(task.Id);

            Assert.IsTrue(result);
            Assert.AreEqual("Maria", updatedTask.Responsible);
            Assert.IsNotNull(updatedTask.UpdatedAt);
            if (oldDate.HasValue)
                Assert.IsTrue(updatedTask.UpdatedAt > oldDate);
        }

        [TestMethod]
        public void UpdateTaskResponsible_TaskDoesNotExist_ReturnsFalse()
        {
            var service = new TaskService(testFolder, testFile);
            var result = service.UpdateTaskResponsible(999, "Maria");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UpdateTaskResponsible_EmptyResponsible_UpdatesToEmpty()
        {
            var service = new TaskService(testFolder, testFile);
            service.CreateTask("Tarea 2", "Desc", "Pedro");
            var task = service.GetTasks()[0];

            var result = service.UpdateTaskResponsible(task.Id, "");

            Assert.IsTrue(result);
            Assert.AreEqual("", service.GetTaskById(task.Id).Responsible);
        }

        [TestMethod]
        public void UpdateTaskResponsible_MultipleUpdates_UpdatesEachTime()
        {
            var service = new TaskService(testFolder, testFile);
            service.CreateTask("Tarea Multi", "Desc", "A");
            var task = service.GetTasks()[0];

            service.UpdateTaskResponsible(task.Id, "B");
            Assert.AreEqual("B", service.GetTaskById(task.Id).Responsible);

            service.UpdateTaskResponsible(task.Id, "C");
            Assert.AreEqual("C", service.GetTaskById(task.Id).Responsible);
        }

        [TestMethod]
        public void UpdateTaskResponsible_NullResponsible_UpdatesToNull()
        {
            var service = new TaskService(testFolder, testFile);
            service.CreateTask("Tarea Null", "Desc", "X");
            var task = service.GetTasks()[0];

            var result = service.UpdateTaskResponsible(task.Id, null);

            Assert.IsTrue(result);
            Assert.IsNull(service.GetTaskById(task.Id).Responsible);
        }

        [TestMethod]
        public void UpdateTaskResponsible_DeletedTask_ReturnsFalse()
        {
            var service = new TaskService(testFolder, testFile);
            service.CreateTask("Tarea Delete", "Desc", "Y");
            var task = service.GetTasks()[0];
            service.DeleteTask(task.Id);

            var result = service.UpdateTaskResponsible(task.Id, "Z");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UpdateTaskResponsible_OnlyUpdatesSpecifiedTask()
        {
            var service = new TaskService(testFolder, testFile);
            service.CreateTask("Tarea 1", "Desc", "Uno");
            service.CreateTask("Tarea 2", "Desc", "Dos");
            var tasks = service.GetTasks();
            var id1 = tasks[0].Id;
            var id2 = tasks[1].Id;

            service.UpdateTaskResponsible(id1, "NuevoUno");

            Assert.AreEqual("NuevoUno", service.GetTaskById(id1).Responsible);
            Assert.AreEqual("Dos", service.GetTaskById(id2).Responsible);
        }

        [TestMethod]
        public void UpdateTaskResponsible_InvalidId_ReturnsFalse()
        {
            var service = new TaskService(testFolder, testFile);
            service.CreateTask("Tarea Inv", "Desc", "Inv");

            Assert.IsFalse(service.UpdateTaskResponsible(0, "X"));
            Assert.IsFalse(service.UpdateTaskResponsible(-1, "Y"));
        }

        [TestMethod]
        public void UpdateTaskResponsible_WhiteSpaceResponsible_UpdatesToWhiteSpace()
        {
            var service = new TaskService(testFolder, testFile);
            service.CreateTask("Tarea Espacio", "Desc", "Alguien");
            var task = service.GetTasks()[0];

            var result = service.UpdateTaskResponsible(task.Id, "   ");

            Assert.IsTrue(result);
            Assert.AreEqual("   ", service.GetTaskById(task.Id).Responsible);
        }

        [TestMethod]
        public void UpdateTaskResponsible_LongResponsible_UpdatesCorrectly()
        {
            var service = new TaskService(testFolder, testFile);
            service.CreateTask("Tarea Larga", "Desc", "Corto");
            var task = service.GetTasks()[0];
            string longName = new string('X', 1000);

            var result = service.UpdateTaskResponsible(task.Id, longName);

            Assert.IsTrue(result);
            Assert.AreEqual(longName, service.GetTaskById(task.Id).Responsible);
        }

        [TestMethod]
        public void UpdateTaskResponsible_SpecialCharacters_UpdatesCorrectly()
        {
            var service = new TaskService(testFolder, testFile);
            service.CreateTask("Tarea Especial", "Desc", "Normal");
            var task = service.GetTasks()[0];
            string special = "!@#$%^&*()_+-=[]{}|;':,.<>/?";

            var result = service.UpdateTaskResponsible(task.Id, special);

            Assert.IsTrue(result);
            Assert.AreEqual(special, service.GetTaskById(task.Id).Responsible);
        }

        [TestMethod]
        public void UpdateTaskResponsible_MiddleTask_UpdatesOnlyThatTask()
        {
            var service = new TaskService(testFolder, testFile);
            for (int i = 0; i < 5; i++)
                service.CreateTask($"Tarea {i}", "Desc", $"Resp{i}");

            var tasks = service.GetTasks();
            var middleId = tasks[2].Id;

            var result = service.UpdateTaskResponsible(middleId, "Central");

            Assert.IsTrue(result);
            Assert.AreEqual("Central", service.GetTaskById(middleId).Responsible);
            Assert.AreEqual("Resp0", service.GetTaskById(tasks[0].Id).Responsible);
            Assert.AreEqual("Resp4", service.GetTaskById(tasks[4].Id).Responsible);
        }

        [TestMethod]
        public void UpdateTaskResponsible_LastTask_UpdatesCorrectly()
        {
            var service = new TaskService(testFolder, testFile);
            for (int i = 0; i < 3; i++)
                service.CreateTask($"Tarea {i}", "Desc", $"Resp{i}");

            var tasks = service.GetTasks();
            var lastId = tasks.Last().Id;

            var result = service.UpdateTaskResponsible(lastId, "Ultimo");

            Assert.IsTrue(result);
            Assert.AreEqual("Ultimo", service.GetTaskById(lastId).Responsible);
        }

        [TestMethod]
        public void UpdateTaskResponsible_FirstTask_UpdatesCorrectly()
        {
            var service = new TaskService(testFolder, testFile);
            for (int i = 0; i < 3; i++)
                service.CreateTask($"Tarea {i}", "Desc", $"Resp{i}");

            var tasks = service.GetTasks();
            var firstId = tasks.First().Id;

            var result = service.UpdateTaskResponsible(firstId, "Primero");

            Assert.IsTrue(result);
            Assert.AreEqual("Primero", service.GetTaskById(firstId).Responsible);
>>>>>>> 192dfe93886e7660d0fbb0c6c3b839b1c6f05943
        }
    }
}