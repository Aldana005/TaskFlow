using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskFlow.Services;

namespace TaskFlow.Services.Tests
{
    [TestClass()]
    public class TaskServiceTests
    {
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
        }
    }
}