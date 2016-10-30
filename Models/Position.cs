public class Position
{
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	
	public bool IsOnGround { get { return Z == 0; } }
}