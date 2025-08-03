using FluentAssertions;
using Profitocracy.Core.Domain.Model.Categories.Factories;

namespace Profitocracy.Core.Tests.Domain.Model;

public class CategoryTests
{
    [Fact]
    public void CreateCategory_ValidValues_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var name = "Housing";
        var plannedAmount = 1500m;

        var category = CategoryFactory.CreateCategory(id, profileId, name, plannedAmount);

        category.Id.Should().Be(id);
        category.ProfileId.Should().Be(profileId);
        category.Name.Should().Be(name);
        category.PlannedAmount.Should().Be(plannedAmount);
    }

    [Fact]
    public void CreateCategory_NullId_ShouldGenerateNewId()
    {
        Guid? id = null;
        var profileId = Guid.NewGuid();
        var name = "Entertainment";
        var plannedAmount = 500m;

        var category = CategoryFactory.CreateCategory(id, profileId, name, plannedAmount);

        category.Id.Should().NotBe(Guid.Empty);
        category.ProfileId.Should().Be(profileId);
        category.Name.Should().Be(name);
        category.PlannedAmount.Should().Be(plannedAmount);
    }

    [Fact]
    public void PlannedAmount_SetNegativeValue_ShouldThrowException()
    {
        var id = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var name = "Entertainment";
        var plannedAmount = -100m;

        Action func = () => CategoryFactory.CreateCategory(id, profileId, name, plannedAmount);
        func.Should().Throw<ArgumentException>()
            .WithMessage("Planned amount cannot be negative.");
    }
}
