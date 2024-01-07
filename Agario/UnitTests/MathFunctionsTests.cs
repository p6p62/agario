using System.Numerics;
using AgarioModels.Game;
using Rectangle = AgarioModels.Game.MathFunctions.Rectangle;

namespace UnitTests
{
  [TestClass]
  public class MathFunctionsTests
  {
    [TestMethod]
    public void TestIsNestedIn()
    {
      // Arrange
      Rectangle outerRectangle = new() { X1 = 0, Y1 = 0, X2 = 10, Y2 = 10 };
      Rectangle innerRectangle = new() { X1 = 2, Y1 = 2, X2 = 8, Y2 = 8 };

      // Act
      bool result = innerRectangle.IsNestedIn(outerRectangle);

      // Assert
      Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestCalculateRadiusByMass()
    {
      // Arrange
      int weight = 10;

      // Act
      float radius = MathFunctions.CalculateRadiusByMass(weight);

      // Assert
      Assert.AreEqual(0.316f, radius, 0.001f);
    }

    [TestMethod]
    public void TestArea()
    {
      // Arrange
      Cell cell = new()
      {
        Position = new Vector2(0, 0),
        Weight = 5
      };

      // Act
      float area = cell.Area;

      // Assert
      Assert.AreEqual(0.157f, area, 0.01f);
    }

    [TestMethod]
    public void TestIsIntersect_RectangleRectangle()
    {
      // Arrange
      Rectangle rectangle1 = new() { X1 = 0, Y1 = 0, X2 = 5, Y2 = 5 };
      Rectangle rectangle2 = new() { X1 = 3, Y1 = 3, X2 = 8, Y2 = 8 };

      // Act
      bool result = MathFunctions.IsIntersect(rectangle1, rectangle2);

      // Assert
      Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestIsIntersect_RectangleCell()
    {
      // Arrange
      Rectangle rectangle = new() { X1 = 0, Y1 = 0, X2 = 5, Y2 = 5 };
      Cell cell = new() { Position = new Vector2(2, 2), Radius = 1 };

      // Act
      bool result = MathFunctions.IsIntersect(rectangle, cell);

      // Assert
      Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestIsIntersect_CellCell()
    {
      // Arrange
      Cell cell1 = new() { Position = new Vector2(0, 0), Radius = 2 };
      Cell cell2 = new() { Position = new Vector2(5, 5), Radius = 2 };

      // Act
      bool result = MathFunctions.IsIntersect(cell1, cell2);

      // Assert
      Assert.IsFalse(result);
    }

    [TestMethod]
    public void TestGetIntersectionArea()
    {
      // Arrange
      Cell cell1 = new() { Position = new Vector2(0, 0), Radius = 2 };
      Cell cell2 = new() { Position = new Vector2(3, 0), Radius = 2 };

      // Act
      float area = MathFunctions.GetIntersectionArea(cell1, cell2);

      // Assert
      Assert.AreEqual(1.8132f, area, 0.001f);
    }

    [TestMethod]
    public void TestCalculateMassCenter()
    {
      // Arrange
      List<Cell> cells = new()
      {
          new Cell { Position = new Vector2(0, 0), Weight = 2 },
          new Cell { Position = new Vector2(2, 0), Weight = 3 }
      };

      // Act
      Vector2 center = MathFunctions.CalculateMassCenter(cells);

      // Assert
      Assert.AreEqual(new Vector2(1.2f, 0), center);
    }

    [TestMethod]
    public void TestGetRectangleCenter()
    {
      // Arrange
      Rectangle rectangle = new() { X1 = 0, Y1 = 0, X2 = 4, Y2 = 4 };

      // Act
      Vector2 center = MathFunctions.GetRectangleCenter(rectangle);

      // Assert
      Assert.AreEqual(new Vector2(2, 2), center);
    }

    [TestMethod]
    public void TestDistanceBetweenCentersSquared()
    {
      // Arrange
      Rectangle rectangle1 = new() { X1 = 0, Y1 = 0, X2 = 2, Y2 = 2 };
      Rectangle rectangle2 = new() { X1 = 3, Y1 = 3, X2 = 5, Y2 = 5 };

      // Act
      float distanceSquared = MathFunctions.DistanceBetweenCentersSquared(rectangle1, rectangle2);

      // Assert
      Assert.AreEqual(18.0f, distanceSquared, 0.01f);
    }

    [TestMethod]
    public void TestIsNestedIn2()
    {
      // Arrange
      Cell outerCell = new() { Position = new Vector2(0, 0), Radius = 5 };
      Cell innerCell = new() { Position = new Vector2(2, 2), Radius = 2 };

      // Act
      bool result = MathFunctions.IsNestedIn(innerCell, outerCell);

      // Assert
      Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestCalculateRadiusByMass2()
    {
      // Arrange
      int mass = 25;

      // Act
      float radius = MathFunctions.CalculateRadiusByMass(mass);

      // Assert
      Assert.AreEqual(0.5f, radius, 0.001f);
    }

    [TestMethod]
    public void TestGetIntersectionArea_NoOverlap()
    {
      // Arrange
      Cell cell1 = new() { Position = new Vector2(0, 0), Radius = 2 };
      Cell cell2 = new() { Position = new Vector2(5, 5), Radius = 2 };

      // Act
      float area = MathFunctions.GetIntersectionArea(cell1, cell2);

      // Assert
      Assert.AreEqual(0, area);
    }

    [TestMethod]
    public void TestGetIntersectionArea_FullyNested()
    {
      // Arrange
      Cell cell1 = new() { Position = new Vector2(0, 0), Radius = 4 };
      Cell cell2 = new() { Position = new Vector2(1, 1), Radius = 2 };

      // Act
      float area = MathFunctions.GetIntersectionArea(cell1, cell2);

      // Assert
      Assert.AreEqual(MathF.PI * 2 * 2, area, 0.001f);
    }

    [TestMethod]
    public void TestDistance()
    {
      // Arrange
      Cell cell1 = new() { Position = new Vector2(0, 0) };
      Cell cell2 = new() { Position = new Vector2(3, 4) };

      // Act
      float distance = MathFunctions.Distance(cell1, cell2);

      // Assert
      Assert.AreEqual(5.0f, distance, 0.01f);
    }

    [TestMethod]
    public void TestCalculateMassCenter_SingleCell()
    {
      // Arrange
      List<Cell> cells = new()
      {
          new Cell { Position = new Vector2(2, 3), Weight = 5 }
      };

      // Act
      Vector2 center = MathFunctions.CalculateMassCenter(cells);

      // Assert
      Assert.AreEqual(new Vector2(2, 3), center);
    }

    [TestMethod]
    public void TestCalculateMassCenter_MultipleCells()
    {
      // Arrange
      List<Cell> cells = new()
    {
        new Cell { Position = new Vector2(1, 2), Weight = 3 },
        new Cell { Position = new Vector2(3, 4), Weight = 2 },
        new Cell { Position = new Vector2(5, 6), Weight = 1 }
    };

      // Act
      Vector2 center = MathFunctions.CalculateMassCenter(cells);

      // Assert
      Vector2 expected = new(2.3333f, 3.3333f);
      Assert.AreEqual(expected.X, center.X, 0.001);
      Assert.AreEqual(expected.Y, center.Y, 0.001);
    }

    [TestMethod]
    public void TestCalculateMassCenter_EmptyList()
    {
      // Arrange
      List<Cell> cells = new();

      // Act
      Vector2 center = MathFunctions.CalculateMassCenter(cells);

      // Assert
      Assert.AreEqual(Vector2.Zero, center);
    }

    [TestMethod]
    public void TestDistance_SamePosition()
    {
      // Arrange
      Cell cell1 = new() { Position = new Vector2(2, 3) };
      Cell cell2 = new() { Position = new Vector2(2, 3) };

      // Act
      float distance = MathFunctions.Distance(cell1, cell2);

      // Assert
      Assert.AreEqual(0.0f, distance);
    }

    [TestMethod]
    public void TestDistance_DifferentPositions()
    {
      // Arrange
      Cell cell1 = new() { Position = new Vector2(1, 2) };
      Cell cell2 = new() { Position = new Vector2(4, 6) };

      // Act
      float distance = MathFunctions.Distance(cell1, cell2);

      // Assert
      Assert.AreEqual(5.0f, distance, 0.01f);
    }

    [TestMethod]
    public void TestDistance_ZeroRadius()
    {
      // Arrange
      Cell cell1 = new() { Position = new Vector2(1, 1), Radius = 0 };
      Cell cell2 = new() { Position = new Vector2(3, 4), Radius = 0 };

      // Act
      float distance = MathFunctions.Distance(cell1, cell2);

      // Assert
      Assert.AreEqual(3.605f, distance, 0.01f);
    }
  }
}