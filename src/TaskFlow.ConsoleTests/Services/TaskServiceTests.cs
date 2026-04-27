using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskFlow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Services.Tests
{
    [TestClass()]
    public class TaskServiceTests
    {
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
        }
    }
}