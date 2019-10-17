using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AmazingCo.Business;
using AmazingCo.Data;
using AmazingCo.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace AmazingCo.L0.Tests
{
    public class NodeBusinessTests
    {
        private readonly Mock<IRepository> _repositoryMock;
        private readonly Mock<IBackgroundWorker> _workerMock;

        public NodeBusinessTests()
        {
            _repositoryMock = new Mock<IRepository>();
            _workerMock = new Mock<IBackgroundWorker>();
        }

        [Fact]
        public void Constructor()
        {
            //Arrange
            Action action1 = () => new NodeBusiness(null, null);
            Action action2 = () => new NodeBusiness(_repositoryMock.Object, null);
            Action action3 = () => new NodeBusiness(null, _workerMock.Object);
            Action action4 = () => new NodeBusiness(_repositoryMock.Object, _workerMock.Object);

            //Assert
            action1.Should().Throw<ArgumentNullException>();
            action2.Should().Throw<ArgumentNullException>();
            action3.Should().Throw<ArgumentNullException>();
            action4.Should().NotThrow();
        }

        [Fact]
        public void ChangeParent_Null_Parameters_ThrowThrow()
        {
            //Arrange
            var business = new NodeBusiness(_repositoryMock.Object, _workerMock.Object);
            Func<Task> action1 = async () => await business.ChangeParentAsync(null, "newParentId");
            Func<Task> action2 = async () => await business.ChangeParentAsync("parentId", null);

            //Assert
            action1.Should().Throw<ArgumentNullException>();
            action2.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangeParent_Node_NotFound_ShouldThrow()
        {
            //Arrange
            Node node = null;
            _repositoryMock
                .Setup(x => x.GetAsync(It.Is<string>(p => p.Equals("nodeId"))))
                .ReturnsAsync(node);

            var business = new NodeBusiness(_repositoryMock.Object, _workerMock.Object);

            //Act
            Func<Task> action = async () => await business.ChangeParentAsync("nodeId", "newParentId");

            //Assert
            action.Should().Throw<ArgumentException>();
            _repositoryMock.VerifyAll();
        }

        [Fact]
        public void ChangeParent_Parent_NotFound_ShouldThrow()
        {
            //Arrange
            Node node = new Node();
            _repositoryMock
                .Setup(x => x.GetAsync(It.Is<string>(p => p.Equals("nodeId"))))
                .ReturnsAsync(node);

            Node parentNode = null;
            _repositoryMock
               .Setup(x => x.GetAsync(It.Is<string>(p => p.Equals("newParentId"))))
               .ReturnsAsync(parentNode);

            var business = new NodeBusiness(_repositoryMock.Object, _workerMock.Object);

            //Act
            Func<Task> action = async () => await business.ChangeParentAsync("nodeId", "newParentId");

            //Assert
            action.Should().Throw<ArgumentException>();
            _repositoryMock.VerifyAll();
        }

        [Fact]
        public async Task ChangeParent_NodeId_Equals_ParentId_Returns()
        {
            //Arrange
            Node node = new Node { Id = "Id"};
            _repositoryMock
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(node);

            Node parentNode = new Node { Id = "Id" };
            _repositoryMock
               .Setup(x => x.GetAsync(It.IsAny<string>()))
               .ReturnsAsync(parentNode);

            var business = new NodeBusiness(_repositoryMock.Object, _workerMock.Object);

            //Act
            await business.ChangeParentAsync("Id", "Id");

            //Assert
            _repositoryMock.VerifyAll();
        }

        [Fact]
        public async Task ChangeParent_ParentId_HasValue_PropagateChanges()
        {
            //Arrange
            Node node = new Node { Id = "nodeId", ParentId = "parentId" };
            _repositoryMock
                .Setup(x => x.GetAsync(It.Is<string>(p => p.Equals("nodeId"))))
                .ReturnsAsync(node);

            Node parentNode = new Node { Id = "newParentId"};
            _repositoryMock
               .Setup(x => x.GetAsync(It.Is<string>(p => p.Equals("newParentId"))))
               .ReturnsAsync(parentNode);

            _workerMock.Setup(x => x.PropagateChanges(It.IsAny<Node>(), It.IsAny<Node>()))
                .Returns(Task.CompletedTask);

            var business = new NodeBusiness(_repositoryMock.Object, _workerMock.Object);

            //Act
            await business.ChangeParentAsync("nodeId", "newParentId");

            //Assert
            Thread.Sleep(10);
            _repositoryMock.VerifyAll();
            _workerMock.VerifyAll();
        }

        [Fact]
        public async Task ChangeParent_ParentId_Null_ChangeRoot()
        {
            //Arrange
            Node node = new Node { Id = "nodeId", ParentId = null };
            _repositoryMock
                .Setup(x => x.GetAsync(It.Is<string>(p => p.Equals("nodeId"))))
                .ReturnsAsync(node);

            Node parentNode = new Node { Id = "newParentId" };
            _repositoryMock
               .Setup(x => x.GetAsync(It.Is<string>(p => p.Equals("newParentId"))))
               .ReturnsAsync(parentNode);

            _repositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Node, bool>>>()))
                .Returns(new List<Node> { new Node() }.AsQueryable());

            _workerMock.Setup(x => x.ChangeRoot(It.IsAny<Node>(), It.IsAny<Node>(), It.IsAny<Node>()))
                .Returns(Task.CompletedTask);

            var business = new NodeBusiness(_repositoryMock.Object, _workerMock.Object);

            //Act
            await business.ChangeParentAsync("nodeId", "newParentId");

            //Assert
            Thread.Sleep(10);
            _repositoryMock.VerifyAll();
            _workerMock.VerifyAll();
        }

        [Fact]
        public void GetChildren_ReturnsList()
        {
            //Arrange
            _repositoryMock
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Node, bool>>>()))
                .Returns(new List<Node> { new Node(), new Node() }.AsQueryable());

            var business = new NodeBusiness(_repositoryMock.Object, _workerMock.Object);

            //Act
            var result = business.GetChildren("nodeId");

            //Assert
            _repositoryMock.VerifyAll();
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);
        }

        [Theory]
        [InlineData("existing", true)]
        [InlineData("notExisting", false)]
        public async Task NodeExists_Returns(string id, bool result)
        {
            //Arrange
            _repositoryMock
                .Setup(x => x.ExistsAsync(It.Is<string>(p => p.Equals(id))))
                .ReturnsAsync(result);

            var business = new NodeBusiness(_repositoryMock.Object, _workerMock.Object);

            //Act
            var actionResult = await business.NodeExists(id);

            //Arrange
            actionResult.Should().Be(result);
        }
    }
}
