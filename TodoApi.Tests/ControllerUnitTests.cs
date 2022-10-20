using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TodoApi.Controllers;
using TodoApi.Models;

namespace TodoApi.Tests
{
    public class ControllerUnitTests
    {
        private readonly Mock<ITodoRepository> _mockRepo;
        private readonly TodoItemsController _controller;
        private readonly IEnumerable<TodoItem> expectedItems = new List<TodoItem>
            {
                new TodoItem { Name = "Walk dog", IsComplete = false, Id = "9e882920-78f2-4aaa-a14a-b062345bc391" },
                new TodoItem { Name = "Walk dog 2", IsComplete = false, Id = "9e882920-78f2-4aaa-a14a-b062345bc392" },
                new TodoItem { Name = "Walk dog 3", IsComplete = true, Id = "9e882920-78f2-4aaa-a14a-b062345bc393" },
                new TodoItem { Name = "Walk dog 4", IsComplete = true, Id = "9e882920-78f2-4aaa-a14a-b062345bc394" }
            };
        private readonly TodoItem expectedItem = new () { Id = "9e882920-78f2-4aaa-a14a-b062345bc395", Name = "Walk dog", IsComplete = false };
        private readonly TodoItem unexpectedItem = new () { Id = "9e882920-78f2-4aaa-a14a-b062345bc396", Name = "Run from Tiger", IsComplete = true };
        private readonly TodoItem emptyItem = new () { };

        public ControllerUnitTests()
        {
            _mockRepo = new Mock<ITodoRepository>();
            _controller = new TodoItemsController(_mockRepo.Object);
        }


        [Test]
        public void GivenARepository_WhenGetAllIsCalledOnTheController_ThenATodoItemTypeIsReturned()
        {
            // Arrange


            // Act
            var result = _controller.GetAll();

            // Assert
            Assert.IsNotNull(result);
            CollectionAssert.IsEmpty(result);
            CollectionAssert.AllItemsAreInstancesOfType(result, expectedItems.GetType());
        }


        [Test]
        public void GivenARepository_WhenGetAllIsCalledOnTheController_ThenItReturnsAllTodoItems()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAll())
                .Returns(expectedItems);

            // Act
            var actionResult = _controller.GetAll();
            IEnumerable<TodoItem> resultList = actionResult.ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            CollectionAssert.IsNotEmpty(actionResult);
            CollectionAssert.AreEqual(expectedItems, actionResult);
            Assert.AreEqual(expectedItems.Count(), resultList.Count());
        }


        [Test]
        [TestCase("9e882920-78f2-4aaa-a14a-b062345bc395")]
        public void GivenARepository_WhenGetByIdIsCalledOnTheController_ThenAProductWithMatchingDetailsIsReturned(string key)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Find(key))
                .Returns(expectedItem);

            // Act
            var actionResult = _controller.GetById(key);
            TodoItem result = ((ObjectResult)actionResult).Value as TodoItem;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedItem, result);
            Assert.IsInstanceOf<ObjectResult>(actionResult);
            Assert.That(result.Id, Is.EqualTo(key));
            Assert.That(result.Name, Is.EqualTo("Walk dog"));
            Assert.That(result.IsComplete, Is.EqualTo(false));
        }


        [Test]
        [TestCase("9e882920-78f2-4aaa-a14a-b062345bc397")]
        public void GivenARepository_WhenGetByIdIsCalledWithInvalidKeyOnTheController_ThenANotFoundIsReturned(string key)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Find(key))
                .Returns(emptyItem);

            // Act
            var actionResult = _controller.GetById(key);
            TodoItem result = ((NotFoundObjectResult)actionResult).Value as TodoItem;

            // Assert
            Assert.IsNull(result);
            Assert.AreEqual(null, result);
            Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
        }


        [Test]
        [TestCase("9e882920-78f2-4aaa-a14a-b062345bc395")]
        public void GivenARepository_WhenDeleteIsCalledOnTheController_ThenAProductWithMatchingDetailsIsReturned(string key)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Remove(key))
                .Returns(expectedItem);

            // Act
            var actionResult = _controller.Delete(key);
            TodoItem result = ((ObjectResult)actionResult).Value as TodoItem;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedItem, result);
            Assert.That(result.Id, Is.EqualTo(key));
            Assert.That(result.Name, Is.EqualTo("Walk dog"));
            Assert.That(result.IsComplete, Is.EqualTo(false));
            Assert.IsInstanceOf<ObjectResult>(actionResult);
        }


        [Test]
        [TestCase("9e882920-78f2-4aaa-a14a-b062345bc396")]
        public void GivenARepository_WhenDeleteIsCalledWithInvalidIdOnTheController_ThenANotFoundIsReturned(string key)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Remove(key))
                .Returns(emptyItem);

            // Act
            var actionResult = _controller.Delete(key);
            //var returnedTodoItem = actionResult;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
        }


        [Test]
        public void GivenARepository_WhenCreateIsCalledOnTheController_ThenTheProductWithMatchingDetailsIsReturned()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Add(expectedItem))
                .Returns(true);

            // Act
            var actionResult = _controller.Create(expectedItem);
            var returnedTodoItem = ((ObjectResult)actionResult).Value as TodoItem;

            // Assert
            Assert.IsNotNull(returnedTodoItem);
            Assert.AreEqual(expectedItem, returnedTodoItem);
            Assert.IsInstanceOf<CreatedAtRouteResult>(actionResult);
        }


        [Test]
        public void GivenARepository_WhenCreateIsCalledWithNullObjectOnTheController_ABadRequestIsReturned()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Add(expectedItem))
                .Returns(null);

            // Act
            var actionResult = _controller.Create(null);
            var returnedTodoItem = ((BadRequestResult)actionResult);

            // Assert
            Assert.IsNotNull(returnedTodoItem);
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }


        [Test]
        [TestCase("9e882920-78f2-4aaa-a14a-b062345bc395")]
        public void GivenARepository_WhenUpdateIsCalledOnTheController_ThenANoContentResultIsReturned(string key)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Find(key))
                .Returns(expectedItem);
            _mockRepo.Setup(repo => repo.Update(expectedItem));

            // Act
            var actionResult = _controller.Update(expectedItem.Id, expectedItem);
            var returnedTodoItem = (NoContentResult)actionResult;

            // Assert
            Assert.IsNotNull(returnedTodoItem);
            Assert.IsInstanceOf<NoContentResult>(actionResult);
        }


        [Test]
        [TestCase("9e882920-78f2-4aaa-a14a-b062345bc396")]
        public void GivenARepository_WhenUpdateIsCalledWithEmptyOnTheController_ThenABadRequestResultIsReturned(string key)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Update(emptyItem));

            // Act
            var actionResult = _controller.Update(key, emptyItem);
            var returnedTodoItem = (BadRequestResult)actionResult;

            // Assert
            Assert.IsNotNull(returnedTodoItem);
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }


        [Test]
        [TestCase("9e882920-78f2-4aaa-a14a-b062345bc396")]
        public void GivenARepository_WhenUpdateIsCalledWithEmptyOnTheController_ThenANotFoundResultIsReturned(string key)
        {
            // Arrange

            // Act
            var actionResult = _controller.Update(key, unexpectedItem);
            var returnedTodoItem = (NotFoundResult)actionResult;

            // Assert
            Assert.IsNotNull(returnedTodoItem);
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }
    }
}
