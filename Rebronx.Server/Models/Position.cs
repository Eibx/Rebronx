public class Position
{
	public int X { get; set; }
	public int Y { get; set; }
	
	public bool IsOnGround { get { return Y == 0; } }
	public bool IsInApartmentBuilding { get { return Y >= 100; } }

	public Position(int x, int y)
	{
		X = x;
		Y = y;
	}
}