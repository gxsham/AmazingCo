using System.Linq;
using System.Threading.Tasks;
using AmazingCo.Data;
using FluentAssertions;
using Xunit;

namespace AmazingCo.L1.Tests
{
    public class RepositoryTests: IClassFixture<DbFixture>
    {
        private readonly IRepository _repository;

        public RepositoryTests(DbFixture dbFixture)
        {
            _repository = new Repository(dbFixture.Context);
        }

        [Fact]
        public async Task GetAsync_Single()
        {
            //Act
            var result = await _repository.GetAsync("a");

            //Assert
            result.Should().NotBeNull();
            result.RootId.Should().Be("root");
            result.Height.Should().Be(1);
            result.ParentId.Should().Be("root");
        }

        [Fact]
        public void GetAsync_Multiple()
        {
            //Act
            var result = _repository.GetAsync(x => x.ParentId == "c").ToList();

            //Assert
            result.Should().HaveCount(2);
            result[0].Id.Should().Be("d");
            result[0].RootId.Should().Be("root");
            result[0].Height.Should().Be(3);
            result[0].ParentId.Should().Be("c");
            result[1].Id.Should().Be("e");
            result[1].RootId.Should().Be("root");
            result[1].Height.Should().Be(3);
            result[1].ParentId.Should().Be("c");
        }

        [Theory]
        [InlineData("a", true)]
        [InlineData("z", false)]
        public async Task ExistsAsync(string id, bool exists)
        {
            //Act
            var result = await _repository.ExistsAsync(id);

            //Assert
            result.Should().Be(exists);
        }

        [Fact]
        public async Task GetSubtree()
        {
            //Act
            var result = await _repository.GetSubtree("a");

            //Assert
            result.Should().HaveCount(3);
        }
    }
}
