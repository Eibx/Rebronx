public class Position
{
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	
	public bool IsOnGround { get { return Z == 0; } }

	public bool Equals(Position obj)
	{		
		if (obj == null) 
		{
			return false;
		}
						
		return (X == obj.X && Y == obj.Y && Z == obj.Z);
	}
}