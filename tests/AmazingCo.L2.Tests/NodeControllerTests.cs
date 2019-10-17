using System.Linq;
using System.Threading.Tasks;
using AmazingCo.Controllers;
using FluentAssertions;
using Xunit;

namespace AmazingCo.L2.Tests
{
    public class NodeControllerTests : IClassFixture<AppFixture>
    {
        private readonly NodesController _controller;

        public NodeControllerTests(AppFixture appFixture)
        {
            _controller = new NodesController(appFixture.nodeBusiness);
        }

        [Fact]
        public async Task Test1()
        {
            //Act
            var result = await _controller.GetChildren("c");

            //Assert
            result.Should().NotBeNull();
            result.Value.Should().HaveCount(2);
            result.Value.First().Id.Should().Be("d");
            result.Value.First().RootId.Should().Be("root");
            result.Value.First().Height.Should().Be(3);
            result.Value.First().ParentId.Should().Be("c");
            result.Value.Last().Id.Should().Be("e");
            result.Value.Last().RootId.Should().Be("root");
            result.Value.Last().Height.Should().Be(3);
            result.Value.Last().ParentId.Should().Be("c");
        }
    }
}
