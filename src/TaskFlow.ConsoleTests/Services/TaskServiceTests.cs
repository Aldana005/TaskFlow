using Microsoft.VisualStudio.TestTools.UnitTesting;
<<<<<<< HEAD
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
using TaskFlow.Services;
=======
using System.Threading.Tasks;
>>>>>>> 192dfe93886e7660d0fbb0c6c3b839b1c6f05943

namespace TaskFlow.Services.Tests
{
    [TestClass()]
    public class TaskServiceTests
    {
<<<<<<< HEAD
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
            }
        }

        [TestMethod]
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
            }
        }

        [TestMethod]
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